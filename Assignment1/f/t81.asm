        s.model  small                  
        .586                           
        .stack  100h                   
        .data                          
x       dw      ?                      
        .code                          
        include io.asm                 
two     proc                           
        push    bp                     
        mov     bp,sp                  
        sub     sp,0                   

        add     sp,0                   
        pop     bp                     
        ret     0                      
two     endp                           

three   proc                           
        push    bp                     
        mov     bp,sp                  
        sub     sp,10                  

        mov     ax,5                   
        mov     [bp-6],ax                

        mov     ax,[bp-6]                
        mov     [bp-2],ax                

        mov     ax,10                  
        mov     [bp-8],ax                

        mov     ax,[bp-8]                
        mov     [bp-4],ax                

        mov     ax,[bp-2]                
        mov     bx,[bp-4]                
        imul    bx                     
        mov     [bp-10],ax                

        mov     ax,[bp-10]                
        mov     bx,[bp+4]                
        mov     [bx],ax                

		mov		ax,[bp+4]
        mov     dx, ax                
        call    writeint                

        add     sp,10                  
        pop     bp                     
        ret     2                      
three   endp                           

one     proc                           
        push    bp                     
        mov     bp,sp                  
        sub     sp,2                   

        mov     ax, offset x                
        push    ax                     

        call    three                  
        add     sp,2                   
        pop     bp                     
        ret     0                      
one     endp                           

main    proc                           
        mov     ax,@data                
        mov     ds,ax                  
        call    one                    
        mov     ah,04ch                
        int     21h                    
main    endp                           
        end     main                   
