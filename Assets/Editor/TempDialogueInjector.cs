using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class TempDialogueInjector
{
    [MenuItem("Tools/Insert Dialogue Into Inspectors")]
    public static void Run()
    {
        string text = @"[1]
Name: Milo
Script: ...Oh.

[2]
Name: Milo
We’ve got a guest.

[3]
Name: Violet
More like a lost one, really.

[4]
Name: Violet
Haven’t seen you round here before.

[5]
Name: Chikitoka
Uh... hi.

[6]
Name: Milo
Hi.

[7]
Name: Violet
Welcome to BandClubTeaTime.

[8]
Name: Chikitoka
BandClub... TeaTime?

Name: Violet
Yeah.
We drink tea, listen to music, read the leaves, and forget things that aren’t worth keeping.

Name: Milo
Mostly food.

Name: Chikitoka
Food?

Name: Milo
We don’t eat.
We just drink tea.

Name: Chikitoka
Is that why you’re both so... thin?

Name: Milo
...

Name: Violet
It’s our aesthetic.

Name: Chikitoka
Right.

Name: Milo
Bit rude, that.

Name: Violet
Oh, leave it.
New people always stare first and think later.

Name: Chikitoka
Sorry.
I wasn’t trying to be rude. I just noticed it.

Name: Violet
Fair enough.
We do tend to make an impression.

Name: Milo
What’s your name, then?

Name: Chikitoka
...

Name: Violet
What is it?

Name: Chikitoka
I don’t remember.

Name: Milo
You forgot your own name?

Name: Chikitoka
Apparently.

Name: Violet
That’s a bit bleak.

Name: Milo
Very on-brand for this place, though.

Name: Violet
Then you definitely need tea.
Names are funny like that. They come back slower than scents.

Name: Milo
I’m Milo.

Name: Violet
And I’m Violet.
Best remember that much, yeah?

Name: Chikitoka
Milo. Violet.

Name: Violet
Now then... what do we call you till your name turns up again?

Name: Milo
That thing on your head.

Name: Chikitoka
What thing?

Name: Milo
Your headset.
Looks like a cup.

Name: Violet
Oh my God, it does.
Like two tiny teacups stuck to your ears.

Name: Milo
There you go, then.
Cubhead.

Name: Chikitoka
Cubhead?

Name: Violet
It’s cute, in a tragic sort of way.

Name: Chikitoka
You just decided that without asking.

Name: Milo
Do you hate it?

Name: Chikitoka
Not exactly.
It’s just weirdly specific.

Name: Violet
That means it suits you.

Name: Milo
Welcome to the club, Cubhead.

Name: Chikitoka
...Thanks, I guess.

Name: Chikitoka
I’m looking for a place.

Chikitoka Archive.
Do you know where it is?

Name: Milo
...

Name: Violet
Straight in with that, then.

Name: Milo
You can’t remember your own name, but you remember that?

Name: Chikitoka
I woke up here with a note in my hand.
It said I had to find Chikitoka Archive.

Name: Violet
A note?

Name: Chikitoka
Yeah.
I don’t remember anything else.
Just that.

Name: Milo
That’s proper ominous.

Name: Violet
Mm.
Then all the more reason not to rush it.

Name: Chikitoka
Why?

Name: Milo
Because this place doesn’t work like that.

Name: Violet
You don’t find your way here by walking about.
You find it through tea.

Name: Chikitoka
Through tea.

Name: Violet
Yes. Through tea.
You drink, we read the leaves, and then we see where you’re meant to go.

Name: Chikitoka
And that actually works?

Name: Milo
Usually.

Name: Violet
More often than maps do.
Especially for people who’ve lost things.

Name: Chikitoka
Names included?

Name: Violet
Names especially.

Name: Chikitoka
So what exactly am I supposed to do?

Name: Violet
Simple.
Pick the tea you want.

Name: Milo
And a cup.

Name: Chikitoka
I have to choose them myself?

Name: Violet
Obviously.
If someone else chooses for you, the reading follows them instead.

Name: Milo
Your tea. Your cup. Your fate.
That’s how it works.

Name: Chikitoka
That sounds suspiciously convenient.

Milo
And yet still true.

Violet
Somewhere in this room, there’s tea and there are teacups.
Go and find one of each.

Milo
And don’t just grab the first thing you see.

Violet
Pick the one that keeps pulling your eyes back.
That’s usually the right one.

Chikitoka
So I choose a tea and a cup, bring them back, and then you read the leaves.

Violet
Exactly.

Chikitoka
And then you’ll tell me where Chikitoka Archive is?

Violet
Perhaps.

Milo
Or why that note wanted you there in the first place.

Chikitoka
...That would also be useful.

Violet
Off you go, then, Cubhead.
Tea doesn’t like waiting.

Milo
And if you take too long, it goes dead.
";

        List<NPCDialogue> list = new List<NPCDialogue>();
        string[] lines = text.Replace("\r", "").Split('\n');
        string currentName = "";

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (string.IsNullOrEmpty(line)) continue;
            if (line.StartsWith("[")) continue;

            if (line.StartsWith("Name: "))
            {
                currentName = line.Substring(6).Trim();
            }
            else if (line.StartsWith("Script: "))
            {
                list.Add(new NPCDialogue() { npcName = currentName, scripts = line.Substring(8).Trim() });
            }
            else if (line == "Milo" || line == "Violet" || line == "Chikitoka") 
            {
                currentName = line;
            }
            else
            {
                // This is a dialogue line
                list.Add(new NPCDialogue() { npcName = currentName, scripts = line });
            }
        }

        NPCController[] npcs = Object.FindObjectsOfType<NPCController>(true);
        if (npcs.Length == 0)
        {
            Debug.LogError("No NPCControllers found in scene!");
            return;
        }

        foreach(var npc in npcs)
        {
            npc.dialogues = new List<NPCDialogue>(list);
            EditorUtility.SetDirty(npc);
            Debug.Log("Successfully inserted " + list.Count + " dialogues into " + npc.name);
        }
        
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
