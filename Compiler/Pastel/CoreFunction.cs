﻿namespace Pastel
{
    internal enum CoreFunction
    {
        NONE,

        // TODO: port this to the standalone Pastel
        GET_FUNCTION,

        ARRAY_GET,
        ARRAY_JOIN,
        ARRAY_LENGTH,
        ARRAY_SET,
        BASE64_TO_STRING,
        CHAR_TO_STRING,
        CHR,
        CURRENT_TIME_SECONDS,
        DICTIONARY_CONTAINS_KEY,
        DICTIONARY_GET,
        DICTIONARY_KEYS,
        DICTIONARY_REMOVE,
        DICTIONARY_SET,
        DICTIONARY_SIZE,
        DICTIONARY_TRY_GET,
        DICTIONARY_VALUES,
        DICTIONARY_NEW,
        EMIT_COMMENT,
        FLOAT_BUFFER_16,
        FLOAT_DIVISION,
        FLOAT_TO_STRING,
        FORCE_PARENS,
        INT,
        INT_BUFFER_16,
        INTEGER_DIVISION,
        INT_TO_STRING,
        IS_VALID_INTEGER,
        LIST_ADD,
        LIST_CLEAR,
        LIST_CONCAT,
        LIST_GET,
        LIST_INSERT,
        LIST_JOIN_STRINGS,
        LIST_JOIN_CHARS,
        LIST_NEW,
        LIST_POP,
        LIST_REMOVE_AT,
        LIST_REVERSE,
        LIST_SET,
        LIST_SHUFFLE,
        LIST_SIZE,
        LIST_TO_ARRAY,
        MATH_ARCCOS,
        MATH_ARCSIN,
        MATH_ARCTAN,
        MATH_COS,
        MATH_LOG,
        MATH_POW,
        MATH_SIN,
        MATH_TAN,
        MULTIPLY_LIST,
        ORD,
        PARSE_FLOAT_UNSAFE,
        PARSE_INT,
        PRINT_STDERR,
        PRINT_STDOUT,
        RANDOM_FLOAT,
        SORTED_COPY_OF_INT_ARRAY,
        SORTED_COPY_OF_STRING_ARRAY,
        STRING_APPEND,
        STRING_BUFFER_16,
        STRING_CHAR_AT,
        STRING_CHAR_CODE_AT,
        STRING_COMPARE_IS_REVERSE,
        STRING_CONCAT_ALL,
        STRING_CONTAINS,
        STRING_ENDS_WITH,
        STRING_EQUALS,
        STRING_FROM_CHAR_CODE,
        STRING_INDEX_OF,
        STRING_LENGTH,
        STRING_REPLACE,
        STRING_REVERSE,
        STRING_SPLIT,
        STRING_STARTS_WITH,
        STRING_SUBSTRING,
        STRING_SUBSTRING_IS_EQUAL_TO,
        STRING_TO_LOWER,
        STRING_TO_UPPER,
        STRING_TRIM,
        STRING_TRIM_END,
        STRING_TRIM_START,
        STRONG_REFERENCE_EQUALITY,
        TRY_PARSE_FLOAT,
    }
}