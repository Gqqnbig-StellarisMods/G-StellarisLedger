grammar Paradox;

//开头是小写的规则是语法规则
//开头是大写的规则是词法规则


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
