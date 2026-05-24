using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PhoneCallInteractable : MonoBehaviour, IConversationTarget
{
    [Serializable]
    public struct CallerTypeChance
    {
        public NpcType NpcType;
        [Range(0f, 1f)] public float Chance;
    }

    [Header("Call Timing")]
    [SerializeField] private float minTimeBetweenCalls = 30f;
    [SerializeField] private float maxTimeBetweenCalls = 60f;

    [Header("Caller Type")]
    [Range(0f, 1f)]
    [SerializeField] private float typedCallChance = 0.5f;
    [SerializeField] private CallerTypeChance[] callerTypeChances =
    {
        new CallerTypeChance { NpcType = NpcType.Karen, Chance = 1f },
        new CallerTypeChance { NpcType = NpcType.RichGuy, Chance = 1f },
        new CallerTypeChance { NpcType = NpcType.ChillGuy, Chance = 1f },
    };

    [Header("Audio")]
    [SerializeField] private AudioClip ringClip;
    [Range(0f, 1f)]
    [SerializeField] private float ringVolume = 1f;

    [Header("Prompt")]
    [SerializeField] private string promptText = "Press E to answer call";

    private ConversationSession conversationSession;
    private ConversationDefinition currentConversation;
    private float callTimer;
    private bool isCallRinging;
    private bool hasAnsweredCall;
    private NpcType? callerType;
    private string callerName;
    private AudioSource ringAudioSource;

    public bool CanInteract => isCallRinging && !hasAnsweredCall;
    public bool ApplyStatusEffects => true;
    public bool UseReviewResolver => true;
    public NpcType? CustomerNpcType => callerType;
    public string ConversationTitleText => callerName;
    public string QuestionText => conversationSession?.CurrentStep?.Question ?? string.Empty;
    public string OptionOneText => GetOptionLabel(0);
    public string OptionTwoText => GetOptionLabel(1);
    public string PromptText => promptText;

    private void Awake()
    {
        EnsureAudioSource();
        ScheduleNextCall();
    }

    private void Update()
    {
        if (hasAnsweredCall || isCallRinging)
        {
            return;
        }

        callTimer -= Time.deltaTime;
        if (callTimer <= 0f)
        {
            BeginIncomingCall();
        }
    }

    public void Interact()
    {
        if (!CanInteract)
        {
            return;
        }

        if (currentConversation == null)
        {
            EndCall();
            return;
        }

        hasAnsweredCall = true;
        isCallRinging = false;
        StopRinging();

        conversationSession = new ConversationSession(currentConversation);
        PlayerInteractionState.SetLocked(true);
        NpcInteractionUI.Instance.Show(this);
    }

    public InteractionOutcome ResolveAnswer(int answerIndex)
    {
        if (conversationSession == null)
        {
            EndCall();
            return new InteractionOutcome
            {
                ConversationFinished = true,
                InteractionResult = InteractionResult.Neutral,
                SuspicionDelta = 0,
                ReviewPointsDelta = 0,
            };
        }

        var finished = conversationSession.ApplyAnswer(answerIndex);
        if (!finished)
        {
            return new InteractionOutcome
            {
                ConversationFinished = false,
                InteractionResult = InteractionResult.Neutral,
                SuspicionDelta = 0,
                ReviewPointsDelta = 0,
            };
        }

        var result = conversationSession.GetInteractionResult();
        var interactionStrength = conversationSession.AccumulatedAnswerValue;
        EndCall();

        return new InteractionOutcome
        {
            ConversationFinished = true,
            InteractionResult = result,
            SuspicionDelta = interactionStrength,
            ReviewPointsDelta = 0,
        };
    }

    private void BeginIncomingCall()
    {
        callerType = RollCallerType();
        currentConversation = PhoneCallConversationDatabase.GetRandomPhoneCallConversation(callerType);

        if (currentConversation == null)
        {
            ScheduleNextCall();
            return;
        }

        callerName = PhoneCallConversationDatabase.GetCallerDisplayName(callerType, PhoneCallConversationDatabase.GetRandomCallerName());
        conversationSession = null;
        isCallRinging = true;
        hasAnsweredCall = false;
        StartRinging();
    }

    private void EndCall()
    {
        StopRinging();

        conversationSession = null;
        currentConversation = null;
        isCallRinging = false;
        hasAnsweredCall = false;
        callerType = null;
        callerName = string.Empty;
        ScheduleNextCall();
    }

    private void ScheduleNextCall()
    {
        var minTime = Mathf.Max(0f, minTimeBetweenCalls);
        var maxTime = Mathf.Max(minTime, maxTimeBetweenCalls);

        callTimer = Random.Range(minTime, maxTime);
    }

    private void EnsureAudioSource()
    {
        ringAudioSource = GetComponent<AudioSource>();
        if (ringAudioSource == null)
        {
            ringAudioSource = gameObject.AddComponent<AudioSource>();
        }

        ringAudioSource.playOnAwake = false;
        ringAudioSource.loop = true;
        ringAudioSource.spatialBlend = 0f;
        ringAudioSource.volume = ringVolume;
    }

    private void StartRinging()
    {
        if (ringAudioSource == null || ringClip == null)
        {
            return;
        }

        ringAudioSource.clip = ringClip;
        ringAudioSource.volume = ringVolume;

        if (!ringAudioSource.isPlaying)
        {
            ringAudioSource.Play();
        }
    }

    private void StopRinging()
    {
        if (ringAudioSource != null && ringAudioSource.isPlaying)
        {
            ringAudioSource.Stop();
        }
    }

    private NpcType? RollCallerType()
    {
        if (Random.value > typedCallChance)
        {
            return null;
        }

        if (callerTypeChances == null || callerTypeChances.Length == 0)
        {
            return null;
        }

        var totalChance = 0f;
        foreach (var entry in callerTypeChances)
        {
            totalChance += Mathf.Max(0f, entry.Chance);
        }

        if (totalChance <= 0f)
        {
            return null;
        }

        var roll = Random.value * totalChance;
        foreach (var entry in callerTypeChances)
        {
            roll -= Mathf.Max(0f, entry.Chance);
            if (roll <= 0f)
            {
                return entry.NpcType;
            }
        }

        return callerTypeChances[callerTypeChances.Length - 1].NpcType;
    }

    private string GetOptionLabel(int index)
    {
        var step = conversationSession?.CurrentStep;
        if (step == null || step.Options == null || index < 0 || index >= step.Options.Length || step.Options[index] == null)
        {
            return string.Empty;
        }

        return step.Options[index].Label;
    }
}