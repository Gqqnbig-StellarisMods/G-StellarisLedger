grammar Paradox;

//开头是小写的规则是语法规则 parser
//开头是大写的规则是词法规则 lexer


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

//在 ANTLR4 中，先写的名称优先级高。
//https://abcdabcd987.com/using-antlr4/
NUMBER
    : '-'?[0-9]+ ('.' [0-9]+)?
    ;

ID : [0-9a-zA-Z_]+ ;

WS : [ \t\r\n]+ -> skip ;
