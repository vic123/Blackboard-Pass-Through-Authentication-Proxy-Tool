echo %JAVA_HOME%
rem "%JAVA_HOME%\bin\keytool" -help > keytool.hlp 2>&1
set CN=PtaProxyTest.com
rem set CN=BBServerTest.com


copy keystore_IdlaJava.jks keystore_IdlaJava_onlyRootCA.jks


keytool -genkey -v -alias %CN% -keyalg RSA -keysize 1024 -keystore keystore_IdlaJava.jks
java  SignCertificate keystore_IdlaJava.jks RootCA_IDLA_Dev_Java %CN% %CN%.signed
keytool -export -alias %CN%.signed -keystore keystore_IdlaJava.jks -file %CN%.signed.crt
keytool -import -alias %CN% -keystore keystore_IdlaJava.jks -file %CN%.signed.crt
keytool -export -alias %CN% -keystore keystore_IdlaJava.jks -file %CN%.crt

openssl x509 -out %CN%-pem.crt -outform pem -in %CN%.crt -inform der
java ExportPriv keystore_IdlaJava.jks %CN% changeit > %CN%-pkcs8.key
rem !! ExportPriv is outputting the base64 private key all on one line. In order to function properly with openssl, you need to ensure that there are no more than 64 printable characters on each line. 
sed "s/.\{64,64\}/&\n/g" %CN%-pkcs8.key > %CN%-pkcs8.split.key
openssl pkcs8 -inform PEM -nocrypt -in %CN%-pkcs8.split.key -out %CN%-rsa.key

openssl pkcs12 -export -out %CN%.pfx -inkey %CN%-rsa.key -in %CN%-pem.crt

