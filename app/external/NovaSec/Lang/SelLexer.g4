lexer grammar SelLexer;

AND: '&&';
OR: '||';
NOT: '!';

TRUE: T R U E;
FALSE: F A L S E;

GT: '>';
LT: '<';
GEQ: '>=';
LEQ: '<=';
EQ: '==';
NEQ: '!=';

LPAREN: '(';
RPAREN: ')';
LBRACKET: '[';
RBRACKET: ']';
COMMA: ',';

DECIMAL: '-'? [0-9]+ ( '.' [0-9]+ )? ;
IDENTIFIER: [A-Za-z_#@]([A-Za-z_0-9.])*;
SPACE: [ \r\t\u000C\n]+ -> channel(HIDDEN);

STRING
 :  '"' ( DOUBLE_ESC_SEQ | ~('\\'|'"') )* '"'
 |  '\'' ( SINGLE_ESC_SEQ | ~('\\'|'\'') )* '\''
 ;

fragment SINGLE_ESC_SEQ
 : '\\\''
 | GENERAL_ESC_SEQ
 ;

fragment DOUBLE_ESC_SEQ
 : '\\\"' 
 | GENERAL_ESC_SEQ
 ;

fragment GENERAL_ESC_SEQ
 :   '\\' ('b'|'t'|'n'|'f'|'r'|'\\')
 |   UNICODE_ESC
 |   OCTAL_ESC
 ;

fragment OCTAL_ESC
 :   '\\' ('0'..'3') ('0'..'7') ('0'..'7')
 |   '\\' ('0'..'7') ('0'..'7')
 |   '\\' ('0'..'7')
 ;

fragment HEX_DIGIT : ('0'..'9'|'a'..'f'|'A'..'F') ;

fragment UNICODE_ESC
  :   '\\' 'u' HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT
  ;

fragment A: [Aa];
fragment B: [Bb];
fragment C: [Cc];
fragment D: [Dd];
fragment E: [Ee];
fragment F: [Ff];
fragment G: [Gg];
fragment H: [Hh];
fragment I: [Ii];
fragment J: [Jj];
fragment K: [Kk];
fragment L: [Ll];
fragment M: [Mm];
fragment N: [Nn];
fragment O: [Oo];
fragment P: [Pp];
fragment Q: [Qq];
fragment R: [Rr];
fragment S: [Ss];
fragment T: [Tt];
fragment U: [Uu];
fragment V: [Vv];
fragment W: [Ww];
fragment X: [Xx];
fragment Y: [Yy];
fragment Z: [Zz];