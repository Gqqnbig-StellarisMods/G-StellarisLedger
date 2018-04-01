grammar Paradox;

paradox:
  kvPair+;

kvPair
  : atom '=' atom
  | atom '=' scope
  | scope
  ;

scope
  : '{' paradox '}'
  | '{' atom+ '}'
  | '{' '}'
  ;

atom
  : STRING
  | NUMBER
  | ID
  ;

STRING
  : '"' (~'"')*? '"'
  ;

ID : [0-9a-zA-Z_]+ ;

NUMBER
    : '-'?[0-9]+
    | '-'?[0-9]* '.' [0-9]+
    ;

WS : [ \t\r\n]+ -> skip ;
