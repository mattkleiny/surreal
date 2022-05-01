grammar Shade;

// statements and declarations
program : declaration* EOF;

// declarations
declaration          : uniformDeclaration | varyingDeclaration | statement;
uniformDeclaration   : 'uniform' primitiveDeclaration ';';
varyingDeclaration   : 'varying' primitiveDeclaration ';';
primitiveDeclaration : (PRECISION)? PRIMITIVE IDENTIFIER;

// statements
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
PRIMITIVE  : 'vec2' | 'vec3' | 'vec4';
PRECISION  : 'lowp' | 'medp' | 'highp';
IDENTIFIER : ([a-zA-Z_])*;
WHITESPACE : (' ' | '\t')+ -> channel(HIDDEN);
