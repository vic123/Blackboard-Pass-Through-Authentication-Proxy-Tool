
echo %JAVA_HOME%
"%JAVA_HOME%\bin\keytool" -genkey -v -alias RootCA_IDLA_Dev_Java -keyalg RSA -keysize 1024 -keystore keystore_IdlaJava.jks
copy keystore_IdlaJava.jks keystore_IdlaJava_onlyRootCA.jks
keytool -export -alias RootCA_IDLA_Dev_Java -keystore keystore_IdlaJava.jks -file RootCA_IDLA_Dev_Java.crt
openssl x509 -out RootCA_IDLA_Dev_Java-pem.crt -outform pem -in RootCA_IDLA_Dev_Java.crt -inform der
java ExportPriv keystore_IdlaJava.jks RootCA_IDLA_Dev_Java changeit > RootCA_IDLA_Dev_Java-pkcs8.key
openssl pkcs8 -inform PEM -nocrypt -in RootCA_IDLA_Dev_Java-pkcs8.key -out RootCA_IDLA_Dev_Java-rsa.key
sed "s/.\{64,64\}/&\n/g" RootCA_IDLA_Dev_Java-pkcs8.key > RootCA_IDLA_Dev_Java-pkcs8.split.key 
openssl pkcs12 -export -out RootCA_IDLA_Dev_Java.pfx -inkey RootCA_IDLA_Dev_Java-pkcs8.split.key -in RootCA_IDLA_Dev_Java-pem.crt

