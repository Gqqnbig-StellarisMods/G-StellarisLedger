grammar ParadoxYaml;

paradoxYaml:
  LANGUAGE_INDICATOR NEWLINE
  (localization? COMMENT? NEWLINE)*
  (localization? COMMENT? EOF)?
  ;

LANGUAGE_INDICATOR: 'l_' [a-z]+ ':';


localization:
  Key Text
  ;

Key:
  [a-zA-Z0-9_]+ ':' [0-9]+
  ;

Text:
  '"'.+?'"'
  ;


COMMENT:
  '#' .+
  ;


NEWLINE: '\r'? '\n';
WS : [ \t]+ -> skip;
