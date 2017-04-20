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