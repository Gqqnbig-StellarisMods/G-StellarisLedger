grammar Paradox;

paradox:
  kvPair+;

kvPair
  : atom '=' atom
  | atom '=' scope
  | scope
  ;

scope
  : '{' (paradox|atom+)? '}'
  ;

atom
  : STRING
  | NUMBER
  | ID (':' ID)?
  ;

STRING
  : '"' (~'"')*? '"'   //~'"' 不是引号
  ;

ID : [0-9a-zA-Z_]+ ;

NUMBER
    : '-'?[0-9]+ ('.' [0-9]+)?
    ;

WS : [ \t\r\n]+ -> skip ;
