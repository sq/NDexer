import win32api
import win32con
import sys
import os
import shutil
import re
from glob import glob

def runProcessGenerator(command):
    streams = os.popen4(command)
    while True:
        line = streams[1].readline()
        if line:
            yield (line, None)
        else:
            break
    exitCode = streams[0].close() or streams[1].close() or 0
    yield (None, exitCode)

def runProcessRealtime(command):
    for (line, exitCode) in runProcessGenerator(command):
        if line:
            sys.stdout.write(line)
        else:
            return exitCode

def runProcess(command):
    result = []
    for (line, exitCode) in runProcessGenerator(command):
        if line:
            result.append(line)
        else:
            return (result, exitCode)

def getRegValue(section, keyName, valueName):
    key = None
    value = None
    try:
        key = win32api.RegOpenKeyEx(section, keyName, 0, win32con.KEY_READ)
        value = win32api.RegQueryValueEx(key, valueName)
    finally:
        if key:
            win32api.RegCloseKey(key)
    return value[0]

def getMsBuildPath():
    return getRegValue(
        win32con.HKEY_LOCAL_MACHINE, 
        r"SOFTWARE\Microsoft\MSBuild\ToolsVersions\3.5",
        "MSBuildToolsPath"
    )

def getSvnVersion(failIfModified=False):
    (result, exitCode) = runProcess("svnversion")
    
    if exitCode != 0:
        for line in result:
            sys.stderr.write(line)
        raise Exception("svnversion.exe returned an error")
    
    version = result[0].strip()
    if version[-1] == 'M':
        raise Exception("Working copy has local modifications")
    elif (':' in version) or ('-' in version):
        raise Exception("Working copy is not at a single revision")
    else:
        version = version[:-1]
    
    return int(version)

def main():
    wcVersion = getSvnVersion()
    
    oldText = open("Version.cs", "r").read()
    open("Version.cs", "w").write(
        re.sub(
            r"(const int Revision = )([0-9]*)(;)",
            lambda m : m.group(1) + str(wcVersion) + m.group(3),
            oldText
        )
    )
    
    print "-- Building NDexer r%d --" % (wcVersion,)
    if runProcessRealtime("%s\msbuild.exe ndexer.sln /v:m /nologo" % (getMsBuildPath(),)) != 0:
        raise Exception("Build failed.")
    
    print "-- Building package --"
    shutil.rmtree(r"dist\temp", True)
    
    os.makedirs(r"dist\temp")
    
    shutil.copy(r"bin\debug\ndexer.exe", r"dist\temp")
    
    for fn in glob(r"bin\debug\*.dll"):
        shutil.copy(fn, r"dist\temp")
    
    print "-- Compressing package --"
    try:
        os.makedirs(r"dist\packages")
    except:
        pass
    
    (result, exitCode) = runProcess(r"ext\7zip\7z.exe a -r -t7z dist\packages\ndexer-r%d.7z .\dist\temp\*.*" % (wcVersion,))
    if exitCode != 0:
        for line in result:
            sys.stdout.write(line)
        raise Exception("Compress failed.")
    
    print r"-- Done. Package built at dist\packages\ndexer-r%d.7z. --" % (wcVersion,)

main()