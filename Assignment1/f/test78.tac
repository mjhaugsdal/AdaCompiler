PROC ONE
_BP-10     = 5
A          = _BP-10
_BP-12     = 10
B          = _BP-12
_BP-14     = A + B
@_BP+6     = _BP-14
_BP-18     =  -A
_BP-16     = 22 + _BP-18
_BP-4      = _BP-16
ENDP ONE       
PROC EIGHT
PUSH @A
PUSH B
CALL ONE
ENDP EIGHT     
START PROC EIGHT
