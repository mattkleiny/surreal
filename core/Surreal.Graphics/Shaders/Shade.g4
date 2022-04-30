grammar Shade;

// statements and declarations
program : declaration* EOF;

declaration         : variableDeclaration | statement;
variableDeclaration : 'var' IDENTIFIER ( '=' expression)? ';';
statement           : expressionStatement | ifStatement;
ifStatement         : 'if' '(' expression ')' statement ('else' statement)?;
expressionStatement : expression ';';

// expressions and sub-expressions
expression : equality;
equality   : comparison ( ('!=' | '==') comparison)*;
comparison : term (( '>' | '<' | '>=' | '<=') term)*;
term       : factor (('+' | '-') factor)*;
factor     : unary ( ( '*' | '/' | '%') unary)*;
unary      : ('-' | '!') unary | primary;

// primary fallback
primary : NUMBER | STRING | 'true' | 'false' | 'nil' | '(' expression ')' | IDENTIFIER;

// core primitive types
NUMBER     : ('0' .. '9')+ ('.' ('0' .. '9')+)?;
STRING     : ('a' .. 'z') | ('A' .. 'Z') | '_';
IDENTIFIER : STRING;
WHITESPACE : (' ' | '\t')+ -> channel(HIDDEN);
