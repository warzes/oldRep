@ECHO off
rem Server start up script
rem -------------------------------------------------------------------------
rem Tries to start the server passed as first argument (LoginServer,
rem WorldServer, MsgrServer), from bin\ or its sub-folders
rem (Release and Debug). If no argument is passed it calls all start bats,
rem to start all servers.
rem -------------------------------------------------------------------------

IF "%1" == "" GOTO NO_ARGS

SET FILENAME=%1

IF NOT EXIST bin\%FILENAME%.exe GOTO SUB_RELEASE
SET PATH=bin\
GOTO RUN

:SUB_RELEASE
IF NOT EXIST bin\Release\%FILENAME%.exe GOTO SUB_DEBUG
SET PATH=bin\Release\
GOTO RUN

:SUB_DEBUG
IF NOT EXIST bin\Debug\%FILENAME%.exe GOTO ERROR
SET PATH=bin\Debug\

:RUN
ECHO Running %FILENAME% from %PATH%
%windir%\system32\ping -n 2 127.0.0.1 > NUL
CLS
CD %PATH%
%FILENAME%.exe
EXIT

:ERROR
ECHO Couldn't find %FILENAME%.exe
PAUSE

EXIT

:NO_ARGS
start start-login
start start-channel
start start-msgr
start start-web