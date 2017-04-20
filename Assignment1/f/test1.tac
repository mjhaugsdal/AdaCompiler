proc        two                     
endp        two                               
proc        three                   
_bp-6       =           5                     
_bp-2       =           _bp-6                 
_bp-8       =           10                    
_bp-4       =           _bp-8                 
_bp-10      =           _bp-2*_bp-4            
_bp-12      =           _bp-10                
@_bp+4      =           _bp-12                
wri                    
@_bp+4                 
wrln                                
endp        three                             
proc        one                     
push        @x                      
call        three                   
endp        one                               
start       proc        one         
