$ full line comment - ignore
CSC_2013     START     0
ONE          BYTE      X'0F'
TWO          BYTE      C'>'
THREE        BYTE      X'26AE'
FOUR         BYTE      C'/-\'
FIVE         RESW      3               $partial line comment ignore
SIX          WORD      97
BUFFER       RESB      512
BUFFEND      EQU       *
SEVEN        EQU       1024
BUFFEND      EQU       *
MAX          EQU       BUFFEND-BUFFER
             END       ONE
