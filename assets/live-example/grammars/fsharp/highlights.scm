[
  "namespace"
  "module"
  "open"
  "type"
  "let"
  "rec"
  "fun"
  "as"
  "in"
  "inline"
  "mutable"
  "of"
  "member"
  "override"
  "default"
  "abstract"
  "inherit"
  "interface"
  "static"
  "val"
  "do"
  "use"
  "new"
  "exception"
  "extern"
  "do!"
  "get" "set" "and"
  "begin" "end"
  "function"
  "delegate"
  "struct"
  "class"
] @keyword

[
  "private"
  "internal"
  "public"
] @keyword.control.access

; Unary prefix operators on a single expression — same semantic role as
; `not`/`nameof`/etc., so they share the @keyword.operator slot rather
; than the generic @keyword used for declaration/control keywords.
["not" "upcast" "downcast" "nameof" "lazy" "assert"] @keyword.operator
; sizeof</typeof</typedefof< are fused keyword+`<` tokens (so the bare words
; stay usable as identifiers); colour the intrinsic node as a keyword.
(type_intrinsic) @keyword.operator

(address_of_expression "&" @operator)
; The `&` combining an and-pattern (`A a & B b`) is a pattern combinator like the
; or-pattern `|`, so it shares the @keyword.control slot rather than @operator.
(and_pattern "&" @keyword.control)
; The `&` combining a flexible-type intersection (`#A & #B`) is a type operator,
; like the tuple-type `*` → @operator.
(type_intersection "&" @operator)
(optional_named_arg "?" @operator)
(deref_expression "!" @operator)
(prefix_bang_expression operator: (bang_op) @operator)
; `!`-led custom operator as a value / name (`(!!)`, `(!%)`) — colour it like
; `(symbolic_op)` so it reads as an operator everywhere, not just in prefix use.
(operator_name (bang_op) @operator)
; Single-char custom operators inside an `(op)` name (`($)`, `(?)`, `(~)`, …) —
; anonymous tokens the global operator-token list below doesn't include, so
; without this they render as plain text between the parens.
(operator_name ["$" "?" "&" "|" "^" "~" "!" "~~~" "*"] @operator)

; `Unchecked.(+)` — the qualified-operator tail `.(+)` is one token.
(operator_member) @operator

(type_constraint ["null" "struct" "comparison" "equality" "unmanaged" "enum" "delegate"] @keyword)
(type_constraint "not" @keyword.operator)
; `or` keyword in heterogeneous SRTP constraints / call-sites:
;   (^a or ^b) : (static member fmap: ^a -> ^b)
;   ((CFunctor or ^b) : (member replace: …) arg)
(type_constraint "or" @keyword)
(srtp_call_expression "or" @keyword)
; Concrete type identifier on the LHS of a heterogeneous constraint /
; call-site (e.g. `CFunctor` in `(CFunctor or ^b)`) — match the
; rest-of-grammar convention of highlighting type names as `@type`.
(type_constraint (long_identifier) @type)
(srtp_call_expression (long_identifier (identifier) @type))

[
  "|"
  "->"
  "if" "then" "else" "elif"
  "match" "with" "when"
  "try" "finally"
  "for" "while" "to" "downto"
  "return" "return!"
  "yield" "yield!"
  "match!"
] @keyword.control

"." @punctuation
[
  "="
  ">"
  "<"
  ">="
  "<="
  "<>"
  "+"
  "-"
  "*"
  "/"
  "%"
  "&&"
  "||"
  "|>"
  "<|"
  ">>"
  "<<"
  "::"
  "~~~"
  "<-"
  ".."
  ":>"
  ":?>"
  ":?"
] @operator

(symbolic_op) @operator

":" @punctuation.delimiter

[
  "("
  ")"
  "["
  "]"
  "[|"
  "|]"
  "{"
  "}"
  "{|"
  "|}"
  "[<"
  ">]"
] @punctuation.bracket

(typed_quotation "<@" @punctuation.special)
(typed_quotation "@>" @punctuation.special)
(untyped_quotation "<@@" @punctuation.special)
(untyped_quotation "@@>" @punctuation.special)

[
  ";"
  ","
] @punctuation.delimiter

(fsi_terminator) @punctuation.delimiter

(preproc_keyword) @keyword.directive
(preproc_if_kw) @keyword.directive
; The inactive `#elif`/`#else`…`#endif` region is one trivia token — colour it
; like a comment (the IDE convention for inactive conditional code).
(preproc_elif_kw) @keyword.directive
(preproc_else_kw) @keyword.directive
(line_directive) @keyword.directive
(preproc_endif_kw) @keyword.directive
; the `#if`/`#elif` condition (`FABLE_COMPILER || …`) is left UNCOLOURED (default)
; — only the directive keyword is highlighted, not the symbols in the condition.
(shebang) @keyword.directive

(line_comment) @comment.line
(xml_doc_comment) @comment.line.documentation
(block_comment) @comment.block
(block_doc_comment) @comment.block.documentation

(int_literal) @constant.numeric.integer
(float_literal) @constant.numeric.float
(char_literal) @constant.character
(string_literal) @string
(verbatim_string) @string
(triple_quoted_string) @string
(interpolated_string) @string
(interpolated_verbatim_string) @string
(interpolated_triple_string) @string
(multidollar_string) @string
(interpolation "{" @punctuation.special)
(interpolation "}" @punctuation.special)
(interpolation (format_string) @string.special)
(interpolation (printf_format_string) @string.special)
(bool_literal) @constant.builtin.boolean
(unit) @constant.builtin
(null_literal) @constant.builtin

; `()` as an empty parameter list (constructor / method / function /
; lambda) isn't the unit *value* — it's just the empty arg-list
; delimiters. Re-capture as @punctuation.bracket so it renders like the
; `(` `)` of `(x)` rather than the unit-literal colour. These come after
; the `(unit)` rule above, so last-capture-in-source-order wins for
; these declaration sites; `()` in value position (`let v = ()`, `f ()`)
; keeps @constant.builtin.
(primary_constructor (unit) @punctuation.bracket)
(parameter (unit) @punctuation.bracket)

(let_binding
  name: (active_pattern_name) @function)

; Active pattern used as a value expression: `(|Foo|)` / `Module.(|Foo|)`.
(active_pattern_expression (active_pattern_name) @function)
(active_pattern_expression (active_pattern_member) @function)

(let_binding
  name: (identifier) @function
  parameters: (parameter
    (identifier) @variable.parameter)*)

(let_binding
  name: (operator_name) @function
  parameters: (parameter
    (identifier) @variable.parameter)*)

(let_decl_indented
  name: (active_pattern_name) @function)

(let_decl_indented
  name: (identifier) @function
  parameters: (parameter
    (identifier) @variable.parameter)*)

(let_decl_indented
  name: (operator_name) @function
  parameters: (parameter
    (identifier) @variable.parameter)*)

; `let x = … in …` (explicit-`in` form) puts the binding name directly on
; let_expression (not a let_decl_indented child), so it needs its own rule.
(let_expression
  name: (identifier) @function
  parameters: (parameter
    (identifier) @variable.parameter)*)

; A lowercase identifier in an `identifier_pattern` is a value BINDING — the
; names introduced by `match`/`function`/`fun`/`let` patterns (`Some v`,
; `Pick [ key ] [ value ]`, `h :: t`). Colour them @variable. The `^[a-z_]`
; guard leaves the Capitalised constructor head (`Some`, `Pick`) to the
; @constructor rule below. Placed BEFORE the let-tuple-destructure rules so
; those still override these to @function for `let (a, b) = …` (last-wins).
((identifier_pattern (long_identifier (identifier) @variable))
 (#match? @variable "^[a-z_]"))

; The `as`-alias binds a name (`… as item`, `Some x as y`) — colour it @variable.
(as_pattern (identifier) @variable)

; Type-annotated for-loop binder name (`for s: string in`, `for k: T, r: T in`) —
; the bare `tuple_typed_pattern` isn't nested in `identifier_pattern`, so colour its
; bound name @variable here. Scoped to `for_expression` so it doesn't affect the
; let-tuple form (`let (a: int, b)`, coloured @function elsewhere).
((for_expression (tuple_typed_pattern pattern: (long_identifier (identifier) @variable)))
 (#match? @variable "^[a-z_]"))

; Typed struct-tuple element binder (`| struct (a: int, b) ->` in a match) —
; same reasoning as the for-binder above. In PARAMETER position the
; parameter-scoped rules further down override this to @variable.parameter.
((struct_tuple_pattern (tuple_typed_pattern pattern: (long_identifier (identifier) @variable)))
 (#match? @variable "^[a-z_]"))

; Tuple-destructured binding names — `let a, b, c = …` (and `let (a, b) = …`).
; Colour them like the single-name binding form so destructured bindings don't
; render uncoloured. The unparenthesized form holds the names as direct
; long_identifiers; the parenthesized form nests them in identifier_pattern.
(let_binding
  name: (unparenthesized_tuple_pattern (long_identifier (identifier) @function)))
(let_decl_indented
  name: (unparenthesized_tuple_pattern (long_identifier (identifier) @function)))
; Parenthesized form nests each element in identifier_pattern; the lowercase
; guard keeps a constructor element (`let (Some a, b) = …`) from being recoloured.
(let_binding
  name: (tuple_pattern (pattern (identifier_pattern (long_identifier (identifier) @function))))
  (#match? @function "^[a-z_]"))
(let_decl_indented
  name: (tuple_pattern (pattern (identifier_pattern (long_identifier (identifier) @function))))
  (#match? @function "^[a-z_]"))
; Typed-first tuple destructuring `let (a: int, b: string) = …` / `let! (a: T, b) = …`.
; Type-annotated elements are `tuple_typed_pattern`; untyped later elements nest
; in identifier_pattern. Colour both like the other destructured binding names.
(let_binding
  name: (tuple_typed_first_pattern
          (tuple_typed_pattern pattern: (long_identifier (identifier) @function)))
  (#match? @function "^[a-z_]"))
(let_binding
  name: (tuple_typed_first_pattern
          (pattern (identifier_pattern (long_identifier (identifier) @function))))
  (#match? @function "^[a-z_]"))
(let_decl_indented
  name: (tuple_typed_first_pattern
          (tuple_typed_pattern pattern: (long_identifier (identifier) @function)))
  (#match? @function "^[a-z_]"))
(let_decl_indented
  name: (tuple_typed_first_pattern
          (pattern (identifier_pattern (long_identifier (identifier) @function))))
  (#match? @function "^[a-z_]"))

(let_and_binding
  name: (identifier) @function
  parameters: (parameter
    (identifier) @variable.parameter)*)

(lambda_expression
  (parameter
    (identifier) @variable.parameter)*)

; `tuple_param` is the per-element parameter shape inside `primary_constructor`
; and (via `tuple_params`) `secondary_constructor` — e.g. `type C(x: int, y: int)`
; and `new (b)`. Highlight the identifier as a parameter — without this it
; falls through to plain text.
(tuple_param (identifier) @variable.parameter)

; Capitalized non-last identifier in a long_identifier — likely a module or
; type segment in a dotted chain (e.g., `Async.FromContinuations`, where
; `Async` is the module). `#match?` ensures only PascalCase identifiers
; match, so `s.ToUpper` doesn't tint `s` as a type. The `.` after `@type`
; requires an immediately-following identifier sibling, so the captured
; identifier is NEVER the last child. Without a leading `.` anchor, this
; matches at any non-last position — so EVERY non-last segment in chains
; like `System.Threading.Interlocked.Increment` gets the @type tint.
((long_identifier (identifier) @type . (identifier))
 (#match? @type "^[A-Z]"))

; …but a VALUE-rooted chain overrides that: when the first segment is a lowercase
; value (`this`, `s`, `myVar`), the following segments are member/property
; accesses, NOT types — e.g. `this.SuffixDelimStart.Length`. Placed AFTER the
; @type rule so Helix's last-capture-in-source-order makes @variable.other.member
; win for these chains. Capitalised-rooted paths (`System.Threading.Foo`) don't
; match here (the root `#match?` fails), so they keep their @type segments.
((long_identifier . (identifier) @_root (identifier) @variable.other.member)
 (#match? @_root "^[a-z_]"))

; Last segment of a multi-segment long_identifier is a member access — e.g.
; `s.ToUpper` parses as long_identifier(s, ToUpper), and `ToUpper` is the
; member. Leading `.` says the first identifier must be the FIRST child;
; trailing `.` says the second identifier must be the LAST child. Between
; them, no anchor — so the second identifier can be at any position after
; the first, which is what catches 3+ segment chains like `Lib.Math.Integer`
; (captures only `Integer`). Single-identifier long_identifiers don't match
; because the pattern requires two identifiers.
;
; This rule sits BEFORE the type/namespace/attribute long_identifier
; captures. Those parent captures cover the whole long_identifier as
; @type/@namespace/@attribute, but tree-sitter's last-in-source-order
; resolution means the inner @variable.other.member here wins for the
; trailing identifier unless a more-specific override re-captures it.
; The explicit overrides further down re-apply @type / @namespace /
; @attribute to the trailing identifier in those contexts.
(long_identifier . (identifier) (identifier) @variable.other.member .)

; In `Module.Path.(|Foo|)` the WHOLE long_identifier is the module/type path
; (the real member is the separate `active_pattern_member`), so EVERY
; capitalised segment is @type — including the last, overriding the
; member-access rule just above. Placed here so last-in-source-order wins.
(active_pattern_expression
  (long_identifier (identifier) @type)
  (#match? @type "^[A-Z]"))

; (Previous PascalCase rule for `dot_expression object: long_identifier` was
; removed — `dot_expression`'s object can no longer be a long_identifier
; after the long_identifier/dot_expression unification refactor, so the
; pattern is impossible. The PascalCase-non-last rule on long_identifier
; above now handles the equivalent case in pure-identifier chains like
; `System.Threading.Interlocked.Increment` directly.)

; Named types anywhere in a type expression
(type_expression (long_identifier) @type)
(generic_type (long_identifier) @type)
(postfix_type (long_identifier) @type)
(flexible_type "#" @punctuation.special)
(flexible_type (long_identifier) @type)
; `T | null` nullable type — `null` is in type position here, so colour it (and
; the operand) as a type rather than as the value-position `null` constant.
(nullable_type (long_identifier) @type)
(nullable_type "null" @type)
(type_parameter) @type.parameter

; Type-provider static arguments: the parameter name of a named arg
; (`CsvProvider<…, Separators=";">`); a named VALUE that is a `[<Literal>]`
; constant referenced by name (literal values keep their own literal scopes).
(static_type_argument name: (identifier) @variable.parameter)
(static_type_argument value: (long_identifier) @constant)

; Unit identifiers in measure types and literals
(measure_power_type (long_identifier) @type)
(measure_expression (long_identifier) @type)
(measure_expression (type_parameter) @type.parameter)

; Type name in :? pattern  (| :? System.Exception as e ->)
(type_check_pattern (long_identifier) @type)
(type_check_pattern (generic_type (long_identifier) @type))
(type_check_pattern (postfix_type (long_identifier) @type))

(attribute_target
  name: (long_identifier) @attribute)
; Attribute target specifier (`[<assembly: …>]`, `[<param: …>]`) — colour the
; specifier word as a keyword (the `return`/`module`/`type` forms are literal
; keyword tokens already covered by the keyword list above).
(attribute_target
  target: (identifier) @keyword)

(namespace_decl
  name: (long_identifier) @namespace)

(module_decl
  name: (long_identifier) @namespace)

; Override: in type/namespace/attribute contexts, the previous rules tinted
; individual identifiers as @variable.other.member or @type (PascalCase
; heuristic). Re-capture EACH identifier as the more-specific @type /
; @namespace / @attribute so the whole long_identifier matches its
; containing context, not the expression-context heuristics.
; (Helix's highlight resolution picks the LAST capture in source order.)
(type_expression (long_identifier (identifier) @type))
(generic_type (long_identifier (identifier) @type))
(postfix_type (long_identifier (identifier) @type))
(flexible_type (long_identifier (identifier) @type))
(nullable_type (long_identifier (identifier) @type))
(measure_power_type (long_identifier (identifier) @type))
(measure_expression (long_identifier (identifier) @type))
(type_check_pattern (long_identifier (identifier) @type))

; In an explicit type-argument application (`Map.empty<string, _>`),
; the `_` is the inference placeholder, not a real type. Re-capture
; with a non-themed name so the @type captures above lose to this
; later match (Helix resolution: last capture in source order) and
; the wildcard renders with the editor's default text color.
; Limited to this construct — `_` in other type positions (e.g.
; `typedefof<_ list>`) keeps the existing @type styling.
((type_application_expression
   (type_expression (long_identifier (identifier) @wildcard)))
  (#eq? @wildcard "_"))
((type_application_expression
   (type_expression (long_identifier) @wildcard))
  (#eq? @wildcard "_"))

; `Type<'T>.StaticMember` — when a generic type application is the OBJECT of a
; member access, a Capitalized head is a type (static-member / nested-type
; access). Restricted to the dot-object position + `^[A-Z]` so generic
; values/functions (`id<int>`, `Map.empty<…>`, `Unchecked.defaultof<int>.X`)
; stay as values — they're camelCase and aren't typed here.
((dot_expression
   object: (type_application_expression
             (long_identifier (identifier) @type)))
  (#match? @type "^[A-Z]"))

; A generic application whose head is a SINGLE Capitalised identifier is a
; generic type / constructor — `ResizeArray<int>()`, `Dictionary<_,_>`,
; `List<int>`. Both `.` anchors restrict this to single-segment heads, so dotted
; heads like `Map.empty<…>` / `Foo.Create<…>` keep their module/member colours;
; `^[A-Z]` keeps generic values/functions (`id<int>`, `box<int>`) uncoloured.
((type_application_expression
   (long_identifier . (identifier) @type .))
  (#match? @type "^[A-Z]"))

(attribute_target name: (long_identifier (identifier) @attribute))
(namespace_decl name: (long_identifier (identifier) @namespace))
(module_decl name: (long_identifier (identifier) @namespace))
(module_decl abbrev: (long_identifier (identifier) @namespace))
(import_decl (long_identifier (identifier) @namespace))
(new_expression (long_identifier (identifier) @type))
(new_expression (generic_type (long_identifier (identifier) @type)))
(object_expression type: (long_identifier (identifier) @type))
(object_expression type: (generic_type (long_identifier (identifier) @type)))

; module M = Lib  /  module M = Lib.Math.Integer  — abbreviation target
(module_decl abbrev: (long_identifier) @namespace)

(import_decl
  [
    (long_identifier) @namespace
  ])

; `this` / `self` / `_` self identifier on instance members. @variable.builtin
; gives themes the option to tint it distinctly from regular bindings (most
; themes pick a slightly off-hue colour for built-in receivers).
(member_self_ident) @variable.builtin

; Underscore-only member-self identifiers (`member _.Foo = …`,
; `member __.Foo = …`, etc.) are placeholders — strip the
; @variable.builtin styling so they render with the editor's default
; text color. `#match?` covers `_`, `__`, `___`, … in one rule.
; Re-capture with a non-themed name so it wins on Helix's
; last-capture-in-source-order resolution.
((member_self_ident (identifier) @wildcard)
  (#match? @wildcard "^_+$"))
((member_self_ident) @wildcard
  (#match? @wildcard "^_+$"))

(member_defn
  name: (identifier) @function)

; Curried member parameters — `member _.M x y = …`, `static member create n = …`.
; Coloured like let-binding params (the let/lambda rules don't cover members, so
; without this a member's curried params render as plain text). Tuple-style
; member params `M(x, y)` are already handled by the `tuple_param` rule above.
(member_defn parameters: (parameter (identifier) @variable.parameter))

(property_accessor (parameter (identifier) @variable.parameter))

; Tuple-destructured parameters: `let f (x, y)`, `member M(a, b: int)`. The
; UNTYPED `(x, y)` form is a `tuple_pattern` (not `tuple_params`), so the rules
; above miss it. Scoped under `parameter` so a tuple pattern in MATCH position
; stays @variable. Lowercase guard keeps a constructor element (`(Some a, b)`)
; from being recoloured.
(parameter
  (tuple_pattern
    (pattern (identifier_pattern (long_identifier (identifier) @variable.parameter))))
  (#match? @variable.parameter "^[a-z_]"))
(parameter
  (tuple_pattern
    (tuple_typed_pattern pattern: (long_identifier (identifier) @variable.parameter))))
; The same tuple destructure, but nested as a `tuple_param` element
; (`(s: int, (r, x): int * float)`) — colour those bindings @variable.parameter too.
((tuple_param
   (tuple_pattern
     (pattern (identifier_pattern (long_identifier (identifier) @variable.parameter)))))
 (#match? @variable.parameter "^[a-z_]"))

; Struct-tuple parameter elements (`let f (struct (a: int, b)) = …`) — the same
; destructure one level deeper, in both the bare and `name: type` element forms.
((parameter
   (tuple_pattern
     (pattern
       (struct_tuple_pattern
         (pattern (identifier_pattern (long_identifier (identifier) @variable.parameter)))))))
 (#match? @variable.parameter "^[a-z_]"))
(parameter
  (tuple_pattern
    (pattern
      (struct_tuple_pattern
        (tuple_typed_pattern pattern: (long_identifier (identifier) @variable.parameter))))))
; …and inside a typed destructure parameter (`(struct (a, b): struct(int * int))`).
((destructure_parameter
   (struct_tuple_pattern
     (pattern (identifier_pattern (long_identifier (identifier) @variable.parameter)))))
 (#match? @variable.parameter "^[a-z_]"))
(destructure_parameter
  (struct_tuple_pattern
    (tuple_typed_pattern pattern: (long_identifier (identifier) @variable.parameter))))

; A PARENTHESIZED tuple pattern as a parameter whose FIRST element is typed
; (`((x: ^t when …, f: int), m)` — tuple_typed_first_pattern nesting): colour
; its typed and bare elements as parameters too.
(parameter
  (tuple_pattern
    (pattern
      (tuple_typed_first_pattern
        (tuple_typed_pattern pattern: (long_identifier (identifier) @variable.parameter))))))
((parameter
   (tuple_pattern
     (pattern
       (tuple_typed_first_pattern
         (pattern (identifier_pattern (long_identifier (identifier) @variable.parameter)))))))
 (#match? @variable.parameter "^[a-z_]"))

; For-loop variable binding (`for item in xs`) — coloured @variable so it (and
; locals-resolved uses of it) read consistently.
(for_expression (identifier) @variable)
; …and the bindings of an unparenthesized tuple binder (`for k, v in pairs`).
(for_expression
  (unparenthesized_tuple_pattern (long_identifier (identifier) @variable)))

(abstract_member_defn
  name: (identifier) @function)

; Member-signature labelled element (`name: T` / `?name: T`): the name reads as
; a parameter; a bare-identifier type reads as a type (postfix/generic/
; type_parameter types are already covered by the standalone type rules above).
(labelled_type name: (identifier) @variable.parameter)
(labelled_type type: (long_identifier (identifier) @type))

(type_constraint member_name: (identifier) @function)
(type_constraint member_name: (operator_name) @function)

(val_field
  name: (identifier) @variable.other.member)

(type_decl
  name: (identifier) @type)

(type_extension_name (identifier) @type)


(type_and_decl
  name: (identifier) @type)

(exception_decl
  name: (identifier) @type)

(type_decl
  alias: (type_expression) @type)

(union_case
  name: (identifier) @constructor)

(union_case_field
  name: (identifier) @variable.other.member)

(named_field_pat
  name: (identifier) @variable.other.member)

(union_case_field type: (long_identifier) @type)

(enum_case
  name: (identifier) @constructor)

(record_type_field
  name: (identifier) @variable.other.member)

(record_field
  name: (long_identifier) @variable.other.member)
; Capture the inner identifier too. This rule comes later in source order
; than the broad @type captures around line 258, so on Helix's last-capture
; resolution the @variable.other.member tint wins for record field names
; instead of falling through to @type via the wrapping long_identifier.
(record_field
  name: (long_identifier (identifier) @variable.other.member))

(record_field_pattern
  name: (long_identifier) @variable.other.member)
; Same inner-identifier override as `record_field` above — last capture in
; source order wins.
(record_field_pattern
  name: (long_identifier (identifier) @variable.other.member))

; Member access on non-identifier expressions: arr.[0].Length, (f x).Name
; After the long_identifier/dot_expression unification, dot_expression only
; fires for compound LHS (index, application, parens, …). In those chains
; every segment is a member access, never a type — so we DON'T re-tint
; nested-member-of-receiver as @type (the previous override rule was
; correct pre-unification when `System.Threading.Interlocked.Increment`
; was nested dot_expressions; that case is now a single long_identifier).
(dot_expression member: (identifier) @variable.other.member)

; Dynamic-access operator `obj?member` — `?` as operator, member as a member ref.
(dynamic_expression "?" @operator)
(dynamic_expression member: (identifier) @variable.other.member)

; DU constructors and active-pattern cases in match patterns.
; Capitalized identifier in identifier_pattern position = constructor (F# convention).
((identifier_pattern
   (long_identifier (identifier) @constructor))
 (#match? @constructor "^[A-Z]"))

; Named DU field pattern: Email(address = addr) → highlight the constructor name
(named_field_pattern constructor: (long_identifier) @constructor)

; Union-case / active-pattern application binder in a for loop:
; `for KeyValue(k, v) in dict do …`. The binder head is the FIRST child of
; `for_expression` (the enumerable always follows `in`), so anchoring on it
; keeps the same @constructor coloring patterns get in a match arm.
((for_expression
   . (long_identifier (identifier) @constructor))
 (#match? @constructor "^[A-Z]"))

; Single-case-union deconstruction in a typed parameter `(Url url: Url)`: the
; FIRST child of `destructure_parameter` is the constructor (the bound name and
; the type follow), so anchoring on it gives the same @constructor colour.
((destructure_parameter
   . (long_identifier (identifier) @constructor))
 (#match? @constructor "^[A-Z]"))
; The deconstructed binding(s) — the long_identifier(s) AFTER the constructor
; (`url` in `(Url url: Url)`) — are param bindings; colour @variable.parameter.
((destructure_parameter
   (long_identifier) (long_identifier (identifier) @variable.parameter))
 (#match? @variable.parameter "^[a-z_]"))

; Same single-case-union deconstruction, but as a tuple-param element
; (`(Wrap inner: int, g)`): the FIRST long_identifier is the constructor, the
; deconstructed binding(s) after it are param bindings.
((tuple_param
   . (long_identifier (identifier) @constructor))
 (#match? @constructor "^[A-Z]"))
((tuple_param
   (long_identifier) (long_identifier (identifier) @variable.parameter))
 (#match? @variable.parameter "^[a-z_]"))

; DU constructor applied as a function in EXPRESSION position — `Keep(string c)`,
; `Keep c`, `Some x`, `Ok value`. Mirrors the pattern-side convention above
; (Capitalised head = constructor); without this, only the pattern side of a
; match arm got @constructor colour and the constructed value did not.
; The head is the FIRST child of `application_expression`; both `.` anchors on
; the long_identifier restrict this to single-segment heads so dotted calls
; (`Array.map`, `Console.WriteLine`) keep their existing colouring.
((application_expression
   . (long_identifier . (identifier) @constructor .))
 (#match? @constructor "^[A-Z]"))

; Lowercase single-segment head of an application — an ordinary function call
; (`isEmpty x`, `equal ()`, `f value`). Mirrors the DU-constructor rule above
; (Capitalised head = @constructor); the `^[a-z_]` guard keeps the two
; disjoint, and both `.` anchors exclude dotted heads (`List.map`,
; `s.ToUpper`) so module/member colouring is unchanged. Bare references
; (`x |> isEmpty`) stay uncoloured — without type info a non-applied
; identifier can't be told apart from a plain value. Placed BEFORE the
; raise/failwith and conversion-operator rules below so their more-specific
; @function.builtin wins on last-capture resolution.
((application_expression
   . (long_identifier . (identifier) @function .))
 (#match? @function "^[a-z_]"))

; Type name in new expressions (not wrapped in type_expression so needs its own capture)
(new_expression (long_identifier) @type)
(new_expression (generic_type (long_identifier) @type))

; Type name in object expressions { new IFoo with … } / { new Base(arg) with … }
(object_expression type: (long_identifier) @type)
(object_expression type: (generic_type (long_identifier) @type))

; Computation expression builder name (async, task, seq, promise, …)
(computation_expression
  builder: (long_identifier) @keyword)

; Oxpecker element-DSL builder (`div() { … }`) — the form WITH an `args` field is a
; function application (an HTML/element tag), not a CE keyword. Placed after the
; rule above so last-match-in-source-order recolours it from @keyword to @function.
(computation_expression
  builder: (long_identifier) @function
  args: _)

; Element-DSL named arguments (HTML attributes / props): the left operand of a
; top-level `=` inside the builder's args — `div(class'="x", id="y")`,
; `button(onClick = handler)`, `For(each=items)`. Single-arg and tuple forms.
; Uses @variable.other.member (Helix's property scope; the theme has no `property`).
(computation_expression
  args: (parenthesized_expression
          (binary_expression left: (long_identifier (identifier) @variable.other.member))))
(computation_expression
  args: (parenthesized_expression
          (tuple_expression
            (binary_expression left: (long_identifier (identifier) @variable.other.member)))))

; Element-DSL fluent chain methods (`div(…).hxTarget("#x").hxSwap("y") { … }`) —
; colour the method name like any other member access.
(computation_expression
  method: (long_identifier (identifier) @variable.other.member))

; Query CE custom operators (select/where/groupBy/join/leftOuterJoin/…).
; The leading keyword on simple query_operators is captured via `op:`;
; compound forms list the literal keywords inside the rule. Tagged
; `@keyword.control` because they direct query progression — same
; semantic role as `for`/`match`/`if` above.
(query_operator op: _ @keyword.control)
(query_join_operator
  ["join" "in" "on"] @keyword.control)
(query_group_by_operator
  ["groupBy" "groupValBy" "groupJoin" "into"] @keyword.control)
(query_left_outer_join_operator
  ["leftOuterJoin" "in" "on" "into"] @keyword.control)

; Type name inside `nameof` — `nameof System.Math` highlights `System.Math` as
; a type, while `nameof xxx` (camelCase value) stays plain. Same `^[A-Z]` guard
; as the raise rule below.
; Per-identifier capture (not just the whole long_identifier) so the
; PascalCase / @variable.other.member heuristics earlier are overridden.
((nameof_expression
   (long_identifier (identifier) @type))
 (#match? @type "^[A-Z]"))

; Exception type after `raise`/`reraise`. Three argument shapes folded
; into one pattern via `[…]` alternatives:
;   raise (MyError args)           — constructor call inside parens
;   raise (MyError)                — no-arg constructor in parens
;   raise MyError                  — no parens
; The `^[A-Z]` guard on the captured identifier avoids false-positives on
; `raise myVar` (variable holding an exception). A PascalCase-named variable
; would still false-positive, but that fights F# naming convention.
;
; This pattern comes BEFORE the @function.builtin pattern below so the
; `raise` long_identifier ends up with @function.builtin colour — Helix
; uses the last matching capture, and capturing `raise` here as
; @function.builtin (no theme colour) would otherwise wipe out the builtin tint.
; Both raise rules capture the INNER identifier (not the wrapping
; long_identifier): the general lowercase-application rule above captures the
; identifier, and an inner capture beats a parent one regardless of source
; order — so the builtin tint must contend on the same node to win (these
; names are single-segment, so the inner identifier spans the same text).
((application_expression
   (long_identifier . (identifier) @function.builtin .)
   [
     (long_identifier) @type
     (parenthesized_expression (long_identifier) @type)
     (parenthesized_expression
       (application_expression
         (long_identifier) @type))
   ])
 (#match? @function.builtin "^(raise|reraise)$")
 (#match? @type "^[A-Z]"))

; Exception-raising functions — highlighted like throw/raise in other languages
((long_identifier . (identifier) @function.builtin .)
 (#match? @function.builtin "^(raise|reraise|failwith|failwithf|invalidArg|invalidOp|nullArg)$"))

; Primitive conversion operators (`string c`, `int x`, `float32 y`, …) applied
; as a function. Unlike raise/failwith above this CANNOT be an unscoped
; text-match — these names double as type names (`x: string`), so matching
; the bare identifier would wrongly tint type annotations too. Anchoring on
; "single-segment head of an application_expression" (same shape as the DU
; constructor rule) restricts the match to actual call sites; type positions
; live under a different parent (type_expression/generic_type/…) and never
; match this pattern, so there's no false positive there.
((application_expression
   . (long_identifier . (identifier) @function.builtin .))
 (#match? @function.builtin "^(byte|sbyte|int8|uint8|int16|uint16|int32|uint32|int64|uint64|int|uint|nativeint|unativeint|float|float32|double|single|decimal|char|string|bigint|enum)$"))

; base and fixed are reserved keywords but appear as plain identifiers in the tree
((identifier) @keyword (#match? @keyword "^(base|fixed)$"))

; CE bang bindings (`let!`/`use!`/`and!`) are now plain `let`/`use`/`and` + an
; immediate `!`, so their bound names are coloured by the `let_binding` /
; `use_binding` rules (no dedicated bang nodes anymore). Colour the trailing `!`
; like the keyword so `let!`/`use!`/`and!` render as a single keyword.
(let_binding "!" @keyword)
(let_decl_indented "!" @keyword)
(let_and_binding "!" @keyword)
(use_binding "!" @keyword)
(use_binding name: (identifier) @variable)
