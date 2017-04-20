PROGRAM     START    0
ALPHA       STX      CHARLIE
            BASE     BAKER
            LDB      #BAKER
            +SUB     CHARLIE,X
BAKER       LDF      #2048
            TD       CHARLIE
CHARLIE     +JEQ     @ALPHA
             END     ALPHA
