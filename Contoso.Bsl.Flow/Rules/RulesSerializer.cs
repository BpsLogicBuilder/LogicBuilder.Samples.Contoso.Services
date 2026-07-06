using Contoso.Bsl.Flow.Rules.Interfaces;
using LogicBuilder.Workflow.Activities.Rules;
using LogicBuilder.Workflow.ComponentModel.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Contoso.Bsl.Flow.Rules
{
    internal class RulesSerializer : IRulesSerializer
    {
        public RuleSet? DeserializeRuleSet(string ruleSetXmlDefinition)
        {
            WorkflowMarkupSerializer serializer = new();
            if (!string.IsNullOrEmpty(ruleSetXmlDefinition))
            {
                using StringReader stringReader = new(ruleSetXmlDefinition);
                using XmlTextReader reader = new(stringReader);
                return serializer.Deserialize(reader) as RuleSet;
            }
            else
            {
                return null;
            }
        }

        public RuleSet? DeserializeRuleSetFile(RulesModule module)
        {
            using StreamReader inStream = new(new MemoryStream(module.RuleSetFile));
            return DeserializeRuleSet(inStream.ReadToEnd());
        }

        public RuleValidation GetValidation(RuleSet ruleSet)
        {
            List<System.Reflection.Assembly> assemblies =
            [
                typeof(LogicBuilder.App.Utils.Interfaces.ITypeHelper).Assembly,
                typeof(LogicBuilder.Forms.Parameters.Expansions.SelectExpandDefinitionParameters).Assembly,
                typeof(Domain.Entities.StudentModel).Assembly,
                typeof(LogicBuilder.RulesDirector.DirectorBase).Assembly
            ];

            RuleValidation ruleValidation = new(typeof(FlowActivity), assemblies);
            if (!ruleSet.Validate(ruleValidation))
            {
                List<string> errors = ruleValidation.Errors.Aggregate
                (
                    new List<string>
                    {
                        string.Format
                        (
                            CultureInfo.CurrentCulture,
                            Properties.Resources.invalidRulesetFormat,
                            ruleSet.Name
                        )
                    },
                    (list, next) =>
                    {
                        list.Add(next.ErrorText);
                        return list;
                    }
                );

                throw new ArgumentException(string.Join(Environment.NewLine, errors));
            }

            return ruleValidation;
        }
    }
}
