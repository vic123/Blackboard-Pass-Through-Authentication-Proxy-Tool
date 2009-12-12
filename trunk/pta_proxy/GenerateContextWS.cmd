@echo off
@rem Syntax: sampleGenClient http://path.to.academic.suite.server
@rem set SERVER to the root url of your Academic Suite server
set SERVER=%1

@rem if sed is not on your path but is on your system, set SED_PATH as appropriate
if not "%SED_PATH%"=="" goto sed_set
set SED_PATH=sed
:sed_set

if "%SERVER%"=="" goto syntax

@rem set DOTNETBIN to wherever your wsdl.exe lives
@rem set DOTNETBIN=D:\Program Files\Microsoft Visual Studio .NET 2003\SDK\v1.1\Bin
set DOTNETBIN=D:\Program Files\Microsoft Visual Studio 8\SDK\v2.0\Bin\

@rem set DOTNETCLIENT_ROOT to the root folder of your .net client application source
set DOTNETCLIENT_ROOT=App_Code

@rem Generated code goes here:
set GENDIR=%DOTNETCLIENT_ROOT%\gen
mkdir %GENDIR%

set TDIR=%DOTNETCLIENT_ROOT%\temp_delete
mkdir %TDIR%
pushd %TDIR%

"%DOTNETBIN%\wsdl" %SERVER%/webapps/ws/services/Context.WS?wsdl
@rem "%DOTNETBIN%\wsdl" %SERVER%/webapps/ws/services/Util.WS?wsdl
@rem "%DOTNETBIN%\wsdl" %SERVER%/webapps/ws/services/Gradebook.WS?wsdl
@rem "%DOTNETBIN%\wsdl" %SERVER%/webapps/ws/services/Content.WS?wsdl

popd

echo MANUALLY Remove duplicate code from %DOTNETCLIENT_ROOT%\temp_delete\UtilWS.cs, GradebookWS.cs, ContentWS.cs and copy into %GENDIR% if the following seds fail

@rem %SED_PATH% "s/System.Web.Services.Protocols.SoapHttpClientProtocol/Microsoft.Web.Services2.WebServicesClientProtocol/" %TDIR%\ContextWS.cs > %GENDIR%\ContextWS.cs

@rem %SED_PATH% "s/public class CourseIdVO/public class util_CourseIdVO/" %TDIR%\UtilWS.cs > %TDIR%\UtilWS.cs_3
@rem %SED_PATH% "s/public class VersionVO/public class util_VersionVO/" %TDIR%\UtilWS.cs_3 > %TDIR%\UtilWS.cs_4
@rem %SED_PATH% "s/System.Web.Services.Protocols.SoapHttpClientProtocol/Microsoft.Web.Services2.WebServicesClientProtocol/" %TDIR%\UtilWS.cs_4 > %GENDIR%\UtilWS.cs

@rem %SED_PATH% "s/public class CourseIdVO/public class gradebook_CourseIdVO/" %TDIR%\GradebookWS.cs > %TDIR%\GradebookWS.cs_3
@rem %SED_PATH% "s/public class VersionVO/public class gradebook_VersionVO/" %TDIR%\GradebookWS.cs_3 > %TDIR%\GradebookWS.cs_4
@rem %SED_PATH% "s/System.Web.Services.Protocols.SoapHttpClientProtocol/Microsoft.Web.Services2.WebServicesClientProtocol/" %TDIR%\GradebookWS.cs_4 > %GENDIR%\GradebookWS.cs

@rem %SED_PATH% "s/public class CourseIdVO/public class content_CourseIdVO/" %TDIR%\ContentWS.cs > %TDIR%\ContentWS.cs_3
@rem %SED_PATH% "s/public class VersionVO/public class content_VersionVO/" %TDIR%\ContentWS.cs_3 > %TDIR%\ContentWS.cs_4
@rem %SED_PATH% "s/System.Web.Services.Protocols.SoapHttpClientProtocol/Microsoft.Web.Services2.WebServicesClientProtocol/" %TDIR%\ContentWS.cs_4 > %GENDIR%\ContentWS.cs

@rem del %TDIR%\*.cs
@rem del %TDIR%\*.cs_2
@rem del %TDIR%\*.cs_3
@rem del %TDIR%\*.cs_4

popd
goto end
:syntax
echo Syntax: %0 http://your.server.url
:end
