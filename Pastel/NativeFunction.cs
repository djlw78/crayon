﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pastel
{
    public enum NativeFunction
    {
        NONE,

        ARRAY_GET,
        ARRAY_SET,
        CHAR_TO_STRING,
        CHR,
        COMMAND_LINE_ARGS,
        CONVERT_RAW_DICTIONARY_VALUE_COLLECTION_TO_A_REUSABLE_VALUE_LIST,
        CURRENT_TIME_SECONDS,
        DICTIONARY_CONTAINS_KEY,
        DICTIONARY_KEYS,
        DICTINOARY_KEYS_TO_VALUE_LIST,
        DICTIONARY_SIZE,
        DICTIONARY_VALUES_TO_VALUE_LIST,
        DICTIONARY_NEW,
        EMIT_COMMENT,
        FLOAT_DIVISION,
        FLOAT_TO_STRING,
        FORCE_PARENS,
        GENERATE_EXCEPTION,
        GET_PROGRAM_DATA,
        GET_RESOURCE_MANIFEST,
        INT,
        INTEGER_DIVISION,
        INT_TO_STRING,
        IS_VALID_INTEGER,
        LIST_GET,
        LIST_NEW,
        LIST_SET,
        LIST_TO_ARRAY,
        MATH_ARCCOS,
        MATH_ARCSIN,
        MATH_ARCTAN,
        MATH_COS,
        MATH_SIN,
        MATH_TAN,
        MULTIPLY_LIST,
        ORD,
        PARSE_FLOAT,
        PARSE_FLOAT_REDUNDANT,
        PARSE_INT,
        PRINT_STDERR,
        PRINT_STDOUT,
        RANDOM_FLOAT,
        READ_BYTE_CODE_FILE,
        SET_PROGRAM_DATA,
        SORTED_COPY_OF_INT_ARRAY,
        SORTED_COPY_OF_STRING_ARRAY,
        STRING_APPEND,
        STRING_COMPARE_IS_REVERSE,
        STRING_CONCAT_ALL,
        STRING_EQUALS,
        STRING_FROM_CHAR_CODE,
        STRING_LENGTH,
        STRONG_REFERENCE_EQUALITY,
        THREAD_SLEEP,

        LIBRARY_FUNCTION,
    }
}
