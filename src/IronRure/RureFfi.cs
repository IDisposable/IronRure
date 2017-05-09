using System;
using System.Runtime.InteropServices;

namespace IronRure
{
    public static class RureFfi
    {
        /// <summary>
        ///   rure_compile compiles the given pattern into a regular expression. The
        ///   pattern must be valid UTF-8 and the length corresponds to the number of
        ///   bytes in the pattern.
        ///  
        ///   flags is a bitfield. Valid values are constants declared with prefix
        ///   RURE_FLAG_.
        ///  
        ///   options contains non-flag configuration settings. If it's NULL, default
        ///   settings are used. options may be freed immediately after a call to
        ///   rure_compile.
        ///  
        ///   error is set if there was a problem compiling the pattern (including if the
        ///   pattern is not valid UTF-8). If error is NULL, then no error information
        ///   is returned. In all cases, if an error occurs, NULL is returned.
        ///  
        ///   The compiled expression returned may be used from multiple threads
        ///   simultaneously.
        /// </summary>
        [DllImport("rure")]
        public static extern IntPtr rure_compile(byte[] pattern, UIntPtr length,
                    uint flags, IntPtr options,
                    IntPtr error);
        
        /// <summary>
        ///   rure_free frees the given compiled regular expression.
        ///  
        ///   This must be called at most once for any rure.
        /// </summary>
        [DllImport("rure")]
        public static extern void rure_free(IntPtr reg);

        /// <summary>
        /// rure_is_match returns true if and only if re matches anywhere in haystack.
        ///
        /// haystack may contain arbitrary bytes, but ASCII compatible text is more
        /// useful. UTF-8 is even more useful. Other text encodings aren't supported.
        /// length should be the number of bytes in haystack.
        ///
        /// start is the position at which to start searching. Note that setting the
        /// start position is distinct from incrementing the pointer, since the regex
        /// engine may look at bytes before the start position to determine match
        /// information. For example, if the start position is greater than 0, then the
        /// \A ("begin text") anchor can never match.
        ///
        /// rure_is_match should be preferred to rure_find since it may be faster.
        ///
        /// N.B. The performance of this search is not impacted by the presence of
        /// capturing groups in your regular expression.
        /// </summary>
        [DllImport("rure")]
        public static extern bool rure_is_match(IntPtr re, byte[] haystack, UIntPtr length,
                                                UIntPtr start);
        
        /// <summary>
        ///   rure_find returns true if and only if re matches anywhere in haystack.
        ///   If a match is found, then its start and end offsets (in bytes) are set
        ///   on the match pointer given.
        ///  
        ///   haystack may contain arbitrary bytes, but ASCII compatible text is more
        ///   useful. UTF-8 is even more useful. Other text encodings aren't supported.
        ///   length should be the number of bytes in haystack.
        ///  
        ///   start is the position at which to start searching. Note that setting the
        ///   start position is distinct from incrementing the pointer, since the regex
        ///   engine may look at bytes before the start position to determine match
        ///   information. For example, if the start position is greater than 0, then the
        ///   \A ("begin text") anchor can never match.
        ///  
        ///   rure_find should be preferred to rure_find_captures since it may be faster.
        ///  
        ///   N.B. The performance of this search is not impacted by the presence of
        ///   capturing groups in your regular expression.
        /// </summary>
        [DllImport("rure")]
        public static extern bool rure_find(IntPtr re, byte[] haystack, UIntPtr length,
                                            UIntPtr start, out RureMatch match);

        /// <summary>
        ///   rure_options_new allocates space for options.
        ///  
        ///   Options may be freed immediately after a call to rure_compile, but otherwise
        ///   may be freely used in multiple calls to rure_compile.
        ///  
        ///   It is not safe to set options from multiple threads simultaneously. It is
        ///   safe to call rure_compile from multiple threads simultaneously using the
        ///   same options pointer.
        /// </summary>
        [DllImport("rure")]
        public static extern IntPtr rure_options_new();
        
        /// <summary>
        ///   rure_options_free frees the given options.
        ///
        ///   This must be called at most once.
        /// </summary>
        [DllImport("rure")]
        public static extern void rure_options_free(IntPtr options);

        /// <summary>
        ///   rure_options_size_limit sets the appoximate size limit of the compiled
        ///   regular expression.
        ///  
        ///   This size limit roughly corresponds to the number of bytes occupied by a
        ///   single compiled program. If the program would exceed this number, then a
        ///   compilation error will be returned from rure_compile.
        /// </summary>
        [DllImport("rure")]
        public static extern void rure_options_size_limit(IntPtr options, UIntPtr limit);

        /// <summary>
        ///   rure_options_dfa_size_limit sets the approximate size of the cache used by
        ///   the DFA during search.
        ///  
        ///   This roughly corresponds to the number of bytes that the DFA will use while
        ///   searching.
        ///  
        ///   Note that this is a *per thread* limit. There is no way to set a global
        ///   limit. In particular, if a regular expression is used from multiple threads
        ///   simultaneously, then each thread may use up to the number of bytes
        ///   specified here.
        /// </summary>
        [DllImport("rure")]
        public static extern void rure_options_dfa_size_limit(IntPtr options, UIntPtr limit);

        /// <summary>
        ///   rure_compile_set compiles the given list of patterns into a single regular
        ///   expression which can be matched in a linear-scan. Each pattern in patterns
        ///   must be valid UTF-8 and the length of each pattern in patterns corresponds
        ///   to a byte length in patterns_lengths.
        ///  
        ///   The number of patterns to compile is specified by patterns_count. patterns
        ///   must contain at least this many entries.
        ///  
        ///   flags is a bitfield. Valid values are constants declared with prefix
        ///   RURE_FLAG_.
        ///  
        ///   options contains non-flag configuration settings. If it's NULL, default
        ///   settings are used. options may be freed immediately after a call to
        ///   rure_compile.
        ///  
        ///   error is set if there was a problem compiling the pattern.
        ///  
        ///   The compiled expression set returned may be used from multiple threads.
        /// </summary>
        [DllImport("rure")]
        public static extern IntPtr rure_compile_set(IntPtr[] patterns,
                                                     UIntPtr[] patterns_lengths,
                                                     UIntPtr patterns_count,
                                                     uint flags,
                                                     IntPtr options,
                                                     IntPtr error);

        /// <summary>
        ///   rure_set_free frees the given compiled regular expression set.
        ///   
        ///   This must be called at most once for any rure_set.
        /// </summary>
        [DllImport("rure")]
        public static extern void rure_set_free(IntPtr re);

// /*
//  * rure_is_match returns true if and only if any regexes within the set
//  * match anywhere in the haystack. Once a match has been located, the
//  * matching engine will quit immediately.
//  *
//  * haystack may contain arbitrary bytes, but ASCII compatible text is more
//  * useful. UTF-8 is even more useful. Other text encodings aren't supported.
//  * length should be the number of bytes in haystack.
//  *
//  * start is the position at which to start searching. Note that setting the
//  * start position is distinct from incrementing the pointer, since the regex
//  * engine may look at bytes before the start position to determine match
//  * information. For example, if the start position is greater than 0, then the
//  * \A ("begin text") anchor can never match.
//  */
// bool rure_set_is_match(rure_set *re, const uint8_t *haystack, size_t length,
//                        size_t start);

// /*
//  * rure_set_matches compares each regex in the set against the haystack and
//  * modifies matches with the match result of each pattern. Match results are
//  * ordered in the same way as the rure_set was compiled. For example,
//  * index 0 of matches corresponds to the first pattern passed to
//  * `rure_compile_set`.
//  *
//  * haystack may contain arbitrary bytes, but ASCII compatible text is more
//  * useful. UTF-8 is even more useful. Other text encodings aren't supported.
//  * length should be the number of bytes in haystack.
//  *
//  * start is the position at which to start searching. Note that setting the
//  * start position is distinct from incrementing the pointer, since the regex
//  * engine may look at bytes before the start position to determine match
//  * information. For example, if the start position is greater than 0, then the
//  * \A ("begin text") anchor can never match.
//  *
//  * matches must be greater than or equal to the number of patterns the
//  * rure_set was compiled with.
//  *
//  * Only use this function if you specifically need to know which regexes
//  * matched within the set. To determine if any of the regexes matched without
//  * caring which, use rure_set_is_match.
//  */
// bool rure_set_matches(rure_set *re, const uint8_t *haystack, size_t length,
//                       size_t start, bool *matches);

        /// <summary>
        /// rure_error_new allocates space for an error.
        /// 
        /// If error information is desired, then rure_error_new should be called
        /// to create an rure_error pointer, and that pointer can be passed to
        /// rure_compile. If an error occurred, then rure_compile will return NULL and
        /// the error pointer will be set. A message can then be extracted.
        /// 
        /// It is not safe to use errors from multiple threads simultaneously. An error
        /// value may be reused on subsequent calls to rure_compile.
        /// </summary>
        [DllImport("rure")]
        public static extern IntPtr rure_error_new();

        /// <summary>
        /// rure_error_free frees the error given.
        ///
        /// This must be called at most once.
        /// </summary>
        [DllImport("rure")]
        public static extern void rure_error_free(IntPtr error);

        /// <summary>
        ///   rure_error_message returns a NUL terminated string that describes the error
        ///   message.
        ///  
        ///   The pointer returned must not be freed. Instead, it will be freed when
        ///   rure_error_free is called. If err is used in subsequent calls to
        ///   rure_compile, then this pointer may change or become invalid.
        /// </summary>
        [DllImport("rure")]
        public static extern IntPtr rure_error_message(IntPtr err);
    }
}
