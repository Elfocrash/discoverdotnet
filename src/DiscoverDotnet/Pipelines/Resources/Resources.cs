﻿using System;
using System.Collections.Generic;
using System.Text;
using Statiq.Common;
using Statiq.Core;
using Statiq.Yaml;

namespace DiscoverDotnet.Pipelines.Resources
{
    public class Resources : Pipeline
    {
        public Resources()
        {
            InputModules = new ModuleList
            {
                new ReadFiles(Config.FromContext(x => x.FileSystem.RootPath.Combine("../../data/resources/*.yml").FullPath)),
                new ExecuteIf(Config.FromContext(x => x.ApplicationState.IsCommand("preview")))
                {
                    new OrderDocuments(Config.FromDocument(x => x.Source)),
                    new TakeDocuments(10)
                }
            };

            ProcessModules = new ModuleList
            {
                new ParseYaml(),
                new ReplaceContent(string.Empty),
                new AddMetadata("Key", Config.FromDocument(x => x.Source.FileNameWithoutExtension.FullPath)),
                new AddMetadata("Link", Config.FromDocument(d => d.GetString("Website"))),
                new AddMetadata("CardType", "Resource"), // TODO: Do we still need this without groups/events?
                new AddMetadata("SearchData", Config.FromDocument(x => x.GetMetadata(
                    "Website",
                    "Title",
                    "Description",
                    "Tags"))),
                new AddMetadata("CardData", Config.FromDocument(x => x.GetMetadata(
                    "Website",
                    "Title",
                    "Image",
                    "Description",
                    "CardType",
                    "Comment",
                    "Tags",
                    "DiscoveryDate")))
            };
        }
    }
}