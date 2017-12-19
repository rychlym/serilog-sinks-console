// Copyright 2017 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Parsing;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog.Sinks.Console.Formatting;

namespace Serilog.Sinks.SystemConsole.Output
{
    /// <summary>
    /// The output template renderer
    /// </summary>
    public class OutputTemplateRenderer : IThemedTextFormatter
    {
        OutputTemplateTokenRenderer[] _renderers;
        readonly MessageTemplate _template;
        readonly IFormatProvider _formatProvider;

        /// <summary>
        ///  Initializes a new instance of the <see cref="OutputTemplateRenderer"/> class.
        /// </summary>
        /// <param name="outputTemplate">The output template.</param>
        /// <param name="formatProvider">The format provider.</param>
        public OutputTemplateRenderer(string outputTemplate, IFormatProvider formatProvider)
        {
            if (outputTemplate == null) throw new ArgumentNullException(nameof(outputTemplate));
            _template = new MessageTemplateParser().Parse(outputTemplate);
            _formatProvider = formatProvider;
        }

        private ConsoleTheme _theme;

        /// <summary>
        /// Gets or sets a console theme.
        /// </summary>
        public ConsoleTheme Theme
        {
            get => _theme;
            set
            {
                if (_theme != value)
                {
                    _theme = value;
                    ApplyTheme();
                }
            } 
        }

        private void ApplyTheme()
        {
            if  (_theme == null)
            {
                // null the theme -> unset also the renderers
                _renderers = null;
                return;
            }

            var renderers = new List<OutputTemplateTokenRenderer>();
            foreach (var token in _template.Tokens)
            {
                if (token is TextToken tt)
                {
                    renderers.Add(new TextTokenRenderer(_theme, tt.Text));
                    continue;
                }

                var pt = (PropertyToken)token;
                if (pt.PropertyName == OutputProperties.LevelPropertyName)
                {
                    renderers.Add(new LevelTokenRenderer(_theme, pt));
                }
                else if (pt.PropertyName == OutputProperties.NewLinePropertyName)
                {
                    renderers.Add(new NewLineTokenRenderer(pt.Alignment));
                }
                else if (pt.PropertyName == OutputProperties.ExceptionPropertyName)
                {
                    renderers.Add(new ExceptionTokenRenderer(_theme, pt));
                }
                else if (pt.PropertyName == OutputProperties.MessagePropertyName)
                {
                    renderers.Add(new MessageTemplateOutputTokenRenderer(_theme, pt, _formatProvider));
                }
                else if (pt.PropertyName == OutputProperties.TimestampPropertyName)
                {
                    renderers.Add(new TimestampTokenRenderer(_theme, pt, _formatProvider));
                }
                else if (pt.PropertyName == "Properties")
                {
                    renderers.Add(new PropertiesTokenRenderer(_theme, pt, _template, _formatProvider));
                }
                else
                {
                    renderers.Add(new EventPropertyTokenRenderer(_theme, pt, _formatProvider));
                }
            }

            _renderers = renderers.ToArray();
        }

        /// <summary>
        /// Formats a log event to an output.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <param name="output">The output text writer.</param>
        public void Format(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            if (output == null) throw new ArgumentNullException(nameof(output));

            if (_theme == null) Theme = ConsoleTheme.None;

            foreach (var renderer in _renderers)
                renderer.Render(logEvent, output);
        }
    }
}
