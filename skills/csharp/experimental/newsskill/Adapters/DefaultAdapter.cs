﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Globalization;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Integration.ApplicationInsights.Core;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Solutions.Middleware;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using NewsSkill.Responses.Main;
using NewsSkill.Services;
using SkillServiceLibrary.Utilities;
using Microsoft.Bot.Solutions.Skills;
using Microsoft.Bot.Builder.Dialogs;

namespace NewsSkill.Adapters
{
    public class DefaultAdapter : BotFrameworkHttpAdapter
    {
        public DefaultAdapter(
            BotSettings settings,
            UserState userState,
            ConversationState conversationState,
            ICredentialProvider credentialProvider,
            TelemetryInitializerMiddleware telemetryMiddleware,
            IBotTelemetryClient telemetryClient)
            : base(credentialProvider)
        {
            OnTurnError = async (context, exception) =>
            {
                CultureInfo.CurrentUICulture = new CultureInfo(context.Activity.Locale ?? "en-us");
                await context.SendActivityAsync(MainStrings.ERROR);
                await context.SendActivityAsync(new Activity(type: ActivityTypes.Trace, text: $"News Skill Error: {exception.Message} | {exception.StackTrace}"));
                telemetryClient.TrackException(exception);

                if (context.IsSkill())
                {
                    // Send and EndOfConversation activity to the skill caller with the error to end the conversation
                    // and let the caller decide what to do.
                    var endOfConversation = Activity.CreateEndOfConversationActivity();
                    endOfConversation.Code = "SkillError";
                    endOfConversation.Text = exception.Message;
                    await context.SendActivityAsync(endOfConversation);
                }
            };

            Use(telemetryMiddleware);

            Use(new TranscriptLoggerMiddleware(new AzureBlobTranscriptStore(settings.BlobStorage.ConnectionString, settings.BlobStorage.Container)));
            Use(new TelemetryLoggerMiddleware(telemetryClient, logPersonalInformation: true));
            Use(new ShowTypingMiddleware());
            Use(new SetLocaleMiddleware(settings.DefaultLocale ?? "en-us"));
            Use(new EventDebuggerMiddleware());
            Use(new SkillMiddleware(userState, conversationState, conversationState.CreateProperty<DialogState>(nameof(DialogState))));
            Use(new SetSpeakMiddleware());
        }
    }
}