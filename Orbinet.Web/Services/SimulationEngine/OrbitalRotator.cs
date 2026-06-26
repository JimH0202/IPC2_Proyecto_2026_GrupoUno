using Orbinet.Web.DataStructures.Matrix;
using Orbinet.Web.Models.Entities;
using Orbinet.Web.Models.Enums;
using Orbinet.Web.Services.SimulationEngine.State;

namespace Orbinet.Web.Services.SimulationEngine
{
    public class OrbitalRotator
    {
        private readonly OrbitNetStore _store;

        // Puedes ajustar estas constantes después sin tocar la lógica central
        private const double EquatorialStepDegrees = 15.0;
        private const double PolarStepDegrees = 30.0;

        private const int EquatorialColumnSlots = 24; // 360 / 15 = 24 posiciones
        private const int PolarRowSlots = 12;         // 360 / 30 = 12 posiciones

        public OrbitalRotator(OrbitNetStore store)
        {
            _store = store;
        }

        public OrbitalRotationResult RotateOneTick()
        {
            OrbitalRotationResult result = new OrbitalRotationResult();

            PendingMovementNode? movementsHead = null;
            PendingMovementNode? movementsTail = null;

            HeaderNode? rowHeader = _store.RedSatellites.GetFirstRowHeader();

            while (rowHeader != null)
            {
                MatrixNode? current = rowHeader.Access;

                while (current != null)
                {
                    result.SatellitesDetected++;

                    OrbitType orbitType = InferOrbitType(current.SatelliteId);

                    SatelliteStateNode state = _store.SatelliteStates.GetOrCreate(
                        current.SatelliteId,
                        current.IpAddress,
                        orbitType,
                        current.Row,
                        current.Column
                    );

                    double nextAngle = CalculateNextAngle(state.OrbitalAngle, state.OrbitType);

                    int targetRow = state.CurrentRow;
                    int targetColumn = state.CurrentColumn;

                    CalculateTargetCoordinates(
                        state,
                        nextAngle,
                        out targetRow,
                        out targetColumn
                    );

                    if (targetRow == state.CurrentRow && targetColumn == state.CurrentColumn)
                    {
                        state.OrbitalAngle = nextAngle;
                        result.UnchangedPositions++;
                    }
                    else
                    {
                        PendingMovementNode movement = new PendingMovementNode
                        {
                            SatelliteId = state.SatelliteId,
                            IpAddress = state.IpAddress,
                            SourceRow = state.CurrentRow,
                            SourceColumn = state.CurrentColumn,
                            TargetRow = targetRow,
                            TargetColumn = targetColumn,
                            StateRef = state,
                            IsValid = true
                        };

                        AppendMovement(ref movementsHead, ref movementsTail, movement);
                    }

                    current = current.Right;
                }

                rowHeader = rowHeader.Next;
            }

            ValidateMovements(movementsHead);

            ApplyMovements(movementsHead, result);

            result.Details =
                "Rotación orbital completada. " +
                "Detectados: " + result.SatellitesDetected +
                ", rotados: " + result.RotatedSuccessfully +
                ", sin cambio visual: " + result.UnchangedPositions +
                ", omitidos por colisión: " + result.SkippedByCollision + ".";

            return result;
        }

        private void AppendMovement(
            ref PendingMovementNode? head,
            ref PendingMovementNode? tail,
            PendingMovementNode movement)
        {
            if (head == null)
            {
                head = movement;
                tail = movement;
                return;
            }

            tail!.Next = movement;
            tail = movement;
        }

        private OrbitType InferOrbitType(string satelliteId)
        {
            if (satelliteId.StartsWith("SAT-POL-"))
            {
                return OrbitType.Polar;
            }

            return OrbitType.Ecuatorial;
        }

        private double CalculateNextAngle(double currentAngle, OrbitType orbitType)
        {
            double step = orbitType == OrbitType.Polar
                ? PolarStepDegrees
                : EquatorialStepDegrees;

            double next = currentAngle + step;

            while (next >= 360.0)
            {
                next -= 360.0;
            }

            while (next < 0.0)
            {
                next += 360.0;
            }

            return next;
        }

        private void CalculateTargetCoordinates(
            SatelliteStateNode state,
            double nextAngle,
            out int targetRow,
            out int targetColumn)
        {
            if (state.OrbitType == OrbitType.Ecuatorial)
            {
                targetRow = state.CurrentRow;

                int columnOffset = ConvertAngleToSlot(nextAngle, EquatorialColumnSlots);
                targetColumn = columnOffset + 1;
            }
            else
            {
                targetColumn = state.CurrentColumn;

                int rowOffset = ConvertAngleToSlot(nextAngle, PolarRowSlots);
                targetRow = rowOffset + 1;
            }
        }

        private int ConvertAngleToSlot(double angle, int slotCount)
        {
            double degreesPerSlot = 360.0 / slotCount;
            int slot = (int)(angle / degreesPerSlot);

            if (slot < 0)
            {
                slot = 0;
            }

            if (slot >= slotCount)
            {
                slot = slotCount - 1;
            }

            return slot;
        }

        private void ValidateMovements(PendingMovementNode? head)
        {
            PendingMovementNode? current = head;

            while (current != null)
            {
                bool duplicatedTarget = ExistsDuplicateTarget(
                    head,
                    current.TargetRow,
                    current.TargetColumn,
                    current
                );

                if (duplicatedTarget)
                {
                    current.IsValid = false;
                    current = current.Next;
                    continue;
                }

                MatrixNode? targetOccupiedNode = _store.RedSatellites.Search(
                    current.TargetRow,
                    current.TargetColumn
                );

                if (targetOccupiedNode != null)
                {
                    bool occupantWillMoveAway = ExistsSourceCoordinate(
                        head,
                        current.TargetRow,
                        current.TargetColumn
                    );

                    if (!occupantWillMoveAway)
                    {
                        current.IsValid = false;
                    }
                }

                current = current.Next;
            }
        }

        private bool ExistsDuplicateTarget(
            PendingMovementNode? head,
            int targetRow,
            int targetColumn,
            PendingMovementNode currentNode)
        {
            PendingMovementNode? current = head;

            while (current != null)
            {
                if (!object.ReferenceEquals(current, currentNode))
                {
                    if (current.TargetRow == targetRow && current.TargetColumn == targetColumn)
                    {
                        return true;
                    }
                }

                current = current.Next;
            }

            return false;
        }

        private bool ExistsSourceCoordinate(PendingMovementNode? head, int row, int column)
        {
            PendingMovementNode? current = head;

            while (current != null)
            {
                if (current.SourceRow == row && current.SourceColumn == column && current.IsValid)
                {
                    return true;
                }

                current = current.Next;
            }

            return false;
        }

        private void ApplyMovements(PendingMovementNode? head, OrbitalRotationResult result)
        {
            // Fase 1: eliminar posiciones anteriores válidas
            PendingMovementNode? current = head;

            while (current != null)
            {
                if (current.IsValid)
                {
                    _store.RedSatellites.Delete(current.SourceRow, current.SourceColumn);
                }
                else
                {
                    if (current.StateRef != null)
                    {
                        current.StateRef.OrbitalAngle = CalculateNextAngle(
                            current.StateRef.OrbitalAngle,
                            current.StateRef.OrbitType
                        );
                    }

                    result.SkippedByCollision++;
                }

                current = current.Next;
            }

            // Fase 2: insertar nuevas posiciones válidas y actualizar estado interno
            current = head;

            while (current != null)
            {
                if (current.IsValid)
                {
                    bool inserted = _store.RedSatellites.Insert(
                        current.TargetRow,
                        current.TargetColumn,
                        current.SatelliteId,
                        current.IpAddress
                    );

                    if (inserted)
                    {
                        if (current.StateRef != null)
                        {
                            current.StateRef.CurrentRow = current.TargetRow;
                            current.StateRef.CurrentColumn = current.TargetColumn;
                            current.StateRef.OrbitalAngle = CalculateNextAngle(
                                current.StateRef.OrbitalAngle,
                                current.StateRef.OrbitType
                            );
                        }

                        result.RotatedSuccessfully++;
                    }
                    else
                    {
                        // Restauración mínima si el insert falla inesperadamente
                        _store.RedSatellites.Insert(
                            current.SourceRow,
                            current.SourceColumn,
                            current.SatelliteId,
                            current.IpAddress
                        );

                        result.SkippedByCollision++;
                    }
                }

                current = current.Next;
            }
        }
    }
}