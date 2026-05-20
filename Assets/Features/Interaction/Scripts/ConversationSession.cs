using System.Collections.Generic;
using UnityEngine;

public class ConversationSession
{
    private readonly ConversationDefinition definition;
    private readonly Dictionary<string, ConversationStep> stepsById = new Dictionary<string, ConversationStep>();
    private string currentStepId;
    private int accumulatedAnswerValue;

    public ConversationSession(ConversationDefinition definition)
    {
        this.definition = definition;
        accumulatedAnswerValue = 0;

        if (definition?.Steps != null)
        {
            foreach (var step in definition.Steps)
            {
                if (step == null || string.IsNullOrWhiteSpace(step.StepId))
                {
                    continue;
                }

                stepsById[step.StepId] = step;
            }
        }

        currentStepId = string.IsNullOrWhiteSpace(definition?.StartStepId) && definition?.Steps != null && definition.Steps.Length > 0 && definition.Steps[0] != null
            ? definition.Steps[0].StepId
            : definition?.StartStepId;
    }

    public ConversationStep CurrentStep => GetStep(currentStepId);
    public string CurrentStepId => currentStepId;
    public int AccumulatedAnswerValue => accumulatedAnswerValue;
    public bool IsFinished => definition == null || string.IsNullOrWhiteSpace(currentStepId) || !stepsById.ContainsKey(currentStepId);

    public bool ApplyAnswer(int answerIndex)
    {
        if (IsFinished)
        {
            return true;
        }

        var currentStep = CurrentStep;
        if (currentStep == null || currentStep.Options == null || answerIndex < 0 || answerIndex >= currentStep.Options.Length)
        {
            return IsFinished;
        }

        var selectedOption = currentStep.Options[answerIndex];
        if (selectedOption == null)
        {
            return IsFinished;
        }

        accumulatedAnswerValue += selectedOption.AnswerValue;
        currentStepId = string.IsNullOrWhiteSpace(selectedOption.NextStepId) ? null : selectedOption.NextStepId;

        return IsFinished;
    }

    public InteractionResult GetInteractionResult()
    {
        if (accumulatedAnswerValue < 0)
        {
            return InteractionResult.Negative;
        }

        if (accumulatedAnswerValue > 0)
        {
            return InteractionResult.Positive;
        }

        return InteractionResult.Neutral;
    }

    private ConversationStep GetStep(string stepId)
    {
        if (string.IsNullOrWhiteSpace(stepId))
        {
            return null;
        }

        stepsById.TryGetValue(stepId, out var step);
        return step;
    }
}