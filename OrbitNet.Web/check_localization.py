import re, glob, xml.etree.ElementTree as ET
keys=set()
for path in glob.glob('Views/**/*.cshtml', recursive=True):
    with open(path, 'r', encoding='utf-8') as f:
        text=f.read()
    keys.update(re.findall(r'Localizer\["([^"\]]+)"\]', text))
    keys.update(re.findall(r'Localizer\[\$"([^"\]]+)"\]', text))
resx_keys=set()
for path in glob.glob('Resources/**/*.resx', recursive=True):
    tree=ET.parse(path)
    for data in tree.findall('.//data'):
        resx_keys.add(data.attrib['name'])
missing=sorted(keys-resx_keys)
print('missing from resx', len(missing))
for k in missing:
    print(k)
