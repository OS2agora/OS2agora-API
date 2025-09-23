parser grammar SelParser;

options { tokenVocab=SelLexer; }

parse
 : expression EOF
 ;

expression
 : LPAREN expression RPAREN										#parenExpression
 | NOT expression												#notExpression
 | left=comparatorinput op=comparator right=comparatorinput		#comparatorExpression
 | function														#functionExpression
 | left=expression AND right=expression							#logicalAndExpression
 | left=expression OR right=expression							#logicalOrExpression
 | bool															#boolExpression
 | IDENTIFIER													#identifierExpression
 ;

comparator
 : GT | GEQ | LT | LEQ | EQ | NEQ
 ;

comparatorinput
 : IDENTIFIER | DECIMAL | STRING
 ;

bool
 : TRUE | FALSE
 ;

function
 : IDENTIFIER LPAREN arguments RPAREN
 ;

arguments
 : (argument (COMMA argument)*)?
 ;

argument
 : IDENTIFIER | list | STRING
 ;

list
 : LBRACKET listitems RBRACKET
 ;

listitems
 : (listitem (COMMA listitem)*)?
 ;

listitem
 : IDENTIFIER | STRING
 ;