rem http://www.grymoire.com/Unix/Sed.html
rem http://www.grymoire.com/Unix/Regular.html
rem http://www.dostips.com/DtCodeSnippets.php
rem sed "s/.\{64,64\}/&\ ^
rem /g" idlatest.com-pkcs8.key

rem sed "s/.\{64,64\}/&\n/g" idlatest.com-pkcs8.key > idlatest.com-pkcs8.split.key
set CN=idlatest.com
openssl pkcs8 -inform PEM -nocrypt -in %CN%-pkcs8.split.key -out %CN%-rsa.key
openssl pkcs12 -export -out %CN%.pfx -inkey %CN%-rsa.key -in %CN%-pem.crt

