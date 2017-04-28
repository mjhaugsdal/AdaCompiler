procedure one is
	procedure two(inout a, b, d : integer) is
	c: integer;
	begin
		c := a * b + d;
		put("The answer is ");
		put(c);
		putln(" ");
	end two;
	procedure three is
	a, b, d : integer;
	begin
		a := 5;
		b := 10;
		d := 20;
		two(b, d, a);
	end three;
begin
end one;