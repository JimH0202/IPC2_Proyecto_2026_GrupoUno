public static class SatelliteMapper
{
    public static SatelliteViewModel ToViewModel(Satellite satellite)
    {
        return new SatelliteViewModel
        {
            Id = satellite.Id,
            Name = satellite.Nombre,
            OrbitId = satellite.OrbitId,
            X = satellite.X,
            Y = satellite.Y,
            IsActive = satellite.IsActive,
            MessagesSent = satellite.MessagesSent,
            MessagesReceived = satellite.MessagesReceived
        };
    }
}  

//Este código define una clase estática `SatelliteMapper` que contiene un método `ToViewModel`. 
// Este método toma un objeto de tipo `Satellite` y lo convierte en un objeto de tipo `SatelliteViewModel`,
// mapeando las propiedades correspondientes. Esto es útil para separar la lógica de negocio de la 
//presentación, permitiendo que el modelo de vista se adapte a las necesidades específicas de la interfaz 
//de usuario sin afectar el modelo de datos subyacente.

