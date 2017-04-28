procedure one is
	procedure two(inout a : integer) is
	b, c : integer;
	begin
	      b := a;
		 b := 2 * b;
	end two;
	procedure three is
	a, b : integer;
	begin
	      b := 5;
		 two(a);
		 putln(a);
	end three;
begin
end one;