grammar Shade;

// expressions and sub-expressions
expression : equality;
equality   : comparison ( ('!=' | '==') comparison)*;
comparison : term (( '>' | '<' | '>=' | '<=') term)*;
term       : factor (('+' | '-') factor)*;
factor     : unary ( ( '*' | '/' | '%') unary)*;
unary      : ('-' | '!') unary | primary;

// primary fallback
primary : NUMBER | STRING | 'true' | 'false' | 'nil' | '(' expression ')';

// core primitive types
NUMBER : ('0' .. '9')+ ('.' ('0' .. '9')+)?;
STRING : ('a' .. 'z') | ('A' .. 'Z') | '_';

WHITESPACE : (' ' | '\t')+ -> channel(HIDDEN);
