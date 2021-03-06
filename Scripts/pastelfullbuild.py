import os
import sys

MSBUILD = r'C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe'
RELEASE_CONFIG = '/p:Configuration=Release'
LIBRARIES_DIR = r'..\Libraries'
INTERPRE_DIR = r'..\Interpreter' # lol, "interpre-dir"

PLATFORMS = [
  'lang-csharp',
  'lang-java',
  'lang-javascript',
  'lang-php',
  'lang-python',
  'csharp-app',
  'java-app',
  'javascript-app',
  'php-server',
  'python-app',
  'javascript-app-android',
  'javascript-app-ios',
]

PLATFORMS_LOOKUP = {}
for p in PLATFORMS:
  PLATFORMS_LOOKUP[p] = True

def run_command(cmd):
  c = os.popen(cmd)
  output = c.read()
  c.close()
  return output

def get_libraries():
  output = []
  for lib_dir in os.listdir(LIBRARIES_DIR):
    d = os.path.join(os.path.abspath(LIBRARIES_DIR), lib_dir)
    pastel_dir = os.path.join(d, 'pastel')
    if os.path.isdir(pastel_dir):
      manifests = []
      for txt in os.listdir(pastel_dir):
        if (txt.endswith('.txt') and PLATFORMS_LOOKUP.get(txt.split('.')[0], False)):
          manifests.append((txt, os.path.join(pastel_dir, txt)))

      output.append({
        'name': lib_dir,
        'manifests': manifests
      })
  return output

def get_interpreter():
  manifests = []
  d = os.path.abspath(INTERPRE_DIR)
  for file in os.listdir(d):
    if file.endswith('.txt') and PLATFORMS_LOOKUP.get(file.split('.')[0], False):
      manifests.append((file, os.path.join(d, file)))
  return {
    'name': "Interpreter",
    'manifests': manifests
  }

def main(args):

  pastelSource = os.environ['PASTEL_SOURCE']

  if pastelSource == None:
    print("PASTEL_SOURCE enironment variable must be set. This should be the root of the https://github.com/blakeohare/pastel repository")
    return

  if len(args) > 1:
    print("Too many args!")
    return

  lib_filter = None
  if len(args) == 1:
    lib_filter = args[0]

  pastel_sln = os.path.join(pastelSource, 'Source', 'Pastel.sln')
  pastel_exe = os.path.join(pastelSource, 'Source', 'bin', 'Release', 'Pastel.exe')
  cmd = MSBUILD + ' ' + os.path.abspath(pastel_sln) + ' ' + RELEASE_CONFIG
  pastel_exe = os.path.abspath(pastel_exe)
  print(run_command(cmd))
  print(cmd)
  things = get_libraries()
  things.append(get_interpreter())
  for lib in things:
    if lib_filter == None or lib_filter.strip().lower() in lib['name'].lower():
      for manifest, path in lib['manifests']:
        print("Exporting: " + lib['name'] + ' --> ' + manifest)
        cmd = pastel_exe + ' ' + path
        print(run_command(cmd))

main(sys.argv[1:])
