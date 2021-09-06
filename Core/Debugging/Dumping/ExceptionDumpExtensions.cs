using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Jay.Reflection;
using Jay.Reflection.Emission;
using Jay.Text;

namespace Jay.Debugging.Dumping
{
    public static class ExceptionDumpExtensions
    {
        private static readonly HashSet<string> _ignoredPropertyNames;

        static ExceptionDumpExtensions()
        {
            _ignoredPropertyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                nameof(Exception.Message),
                nameof(Exception.HResult),
                nameof(Exception.HelpLink),
                nameof(Exception.Source),
                nameof(Exception.TargetSite),
                nameof(Exception.Data),
                nameof(Exception.InnerException),
                nameof(AggregateException.InnerExceptions),
            };
        }
        
        public static TextBuilder AppendDump(this TextBuilder textBuilder, 
                                             Exception? exception,
                                             DumpOptions? options = default)
        {
            if (!Dumper.CheckNull(textBuilder, exception))
                return textBuilder;
            var exceptionType = exception.GetType();
            textBuilder.AppendDump(exceptionType, options).Append(':').AppendNewLine();
            // Message
            textBuilder.Append("-Message: ").Append(exception.Message).AppendNewLine();

            // HResult
            textBuilder.Append("-HResult: 0x").AppendFormat(exception.HResult, "X");
            
            // Help Link
            if (!string.IsNullOrWhiteSpace(exception.HelpLink))
            {
                textBuilder.Append("-HelpLink: ")
                           .Append(exception.HelpLink)
                           .Terminate(Environment.NewLine);
            }
            
            // Source
            if (!string.IsNullOrWhiteSpace(exception.Source))
            {
                textBuilder.Append("-Source: ")
                           .Append(exception.Source)
                           .Terminate(Environment.NewLine);
            }
            
            // Target Site
            if (exception.TargetSite != null)
            {
                textBuilder.Append("-TargetSite: ")
                           .AppendDump(exception.TargetSite, options)
                           .AppendNewLine();
            }
                     
            // Data
            var data = exception.Data;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (data != null)
            {
                if (data.Count > 0)
                {
                    textBuilder.Append("-Data:").AppendNewLine()
                               .AppendDelimit(Environment.NewLine,
                                              data.AsDictionaryEnumerable(),
                                              (tb, entry) => tb.Append(" |").AppendDump(entry.Key, options).Append(": ")
                                                               .AppendDump(entry.Value, options));
                }
            }

            if (exception is AggregateException aggregateException)
            {
                if (aggregateException.InnerExceptions.Count > 0)
                {
                    textBuilder.Append("-InnerExceptions:").AppendNewLine()
                               .AppendDelimit(Environment.NewLine, aggregateException.InnerExceptions, (tb, ex) =>
                               {
                                   tb.AppendDump(ex).TrimEnd();
                               });
                }

                if (aggregateException.InnerException != null)
                {
                    Debugger.Break();
                }
            }
            else
            {
                // Inner Exception
                if (exception.InnerException != null)
                {
                    textBuilder.Append("-InnerException:").AppendNewLine()
                               .AppendDump(exception.InnerException)
                               .Terminate(Environment.NewLine);
                }
            }
            
            // Stack Trace
            if (!string.IsNullOrWhiteSpace(exception.StackTrace))
            {
                textBuilder.Append("-StackTrace:").AppendNewLine()
                           .Append(exception.StackTrace)
                           .TrimEnd().Terminate(Environment.NewLine);
            }
            
            // Other properties?
            var properties = exceptionType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                          .Where(property => !_ignoredPropertyNames.Contains(property.Name));
            foreach (var property in properties)
            {
                object? value = property.GetValue<Exception, object?>(ref exception);
                if (value != null)
                {
                    textBuilder.Append("-")
                               .Append(property.Name)
                               .Append(": ")
                               .AppendDump(value)
                               .AppendNewLine();
                }
            }

            return textBuilder;
        }
    }
}