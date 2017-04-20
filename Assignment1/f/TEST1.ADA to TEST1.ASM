//test1.ada
procedure one is
  x:integer;
    procedure two is
  begin
  end two;
  
  procedure three(out c:integer) is
  a,b:integer;
  begin
    a:=5;
    b:=10;
    c:=a*b;
    put(c);
  end three;
begin
  three(x);
end one;
//test1.tac
proc two

endp two

proc three
_bp-2 = 5
_bp-4 = 10
_bp-6 = _bp-2 * _bp-4
@_bp+4 = _bp-6
wri @_bp+4
wrln
endp three

proc    one
push @x
call three
endp  	one
start proc one
//test1.asm
        .model small
        .stack 100h
        .data
x		dw		?
        .code
        include io.asm
main   	PROC
        mov ax, @data
        mov ds, ax
        call one
        mov ah, 4ch
        int 21h
main 	ENDP
two     PROC
        push bp
        mov bp, sp
        sub sp, 0
        
        add sp, 0
        pop bp
        ret 0
two		endp
three   PROC
        push bp
        mov bp, sp
        sub sp, 6  // size of locals
       
	   _bp-2 = 5 // wrong
			
	   _bp-6 = 5 // correct                     
	   _bp-2 = _bp-6     
	   
	   
	    //right code
		mov ax, 5
		mov [bp-6], ax
		
		mov	bx, [bp-6]
		mov [bp-2], bx //bp-2 now has 5!
	   
	   
	   
		//wrong code
  	    mov ax, 5  // code starts here
        mov [bp-2], ax
		
		
		
		
		
        mov ax, 10
        mov [bp-4], ax
        mov ax, [bp-2]
        mov bx, [bp-4]
        imul bx
        mov [bp-6], ax
        mov ax, [bp-6]
		mov bx,[bp+4]
        mov [bx],ax
        mov dx, [bp-6]
        call writeint
        mov ax, [bp-6] // code ends here
			
        add sp, 6
        pop bp
        ret 2
three	endp
one		PROC
		mov	 ax, offset x
		push ax
        call three
        ret
one		endp
		END main 