# AdjectivestVB
VB.NET 2010 Ultimate Conversion of Samuel Emaus Adjectivest.  

This is a simple Windows Form project.  Run it with Ctrl-F5, and hit the button.

Tests have been written, which document how to use the code.  Coverage is set up in the solution also.

The conversion was partly done automatically, so is fairly rough and ready.  It seems to work, but no guarantees.  I've added a few extra comments, plus a fix in GetInflectionType() to leSuffix processing. 

The original readme is as follows:

## Adjectivest

This is a small library for inflecting Adjectives in English between their comparative, superlative, and base forms.

I was working with some NLP libraries in .NET and couldn't find something like this, so I took it upon myself to put this together.

Essentially, if you're working with NLP and have any kind of need to inflect English adjectives, this should cover your need. 

## How to Use:

Clone the repo wherever you'd like:

```
git clone https://github.com/samuelemaus/Adjectivest
```

### Option A) Include Adjectivest in your Solution

#### Installing
______________

*This is assuming you are using Visual Studio.*

In your Solution, right-click on the Solution in the Solution Explorer.  Highlight **'Add'** and select **'Existing Project'**.  Then select the **Adjectivest.csproj** file in the **Adjectivest** directory of wherever you cloned the repository.

You can then reference the Project however you'd like.  When you build your solution, a folder called **'AdjectivestResources'** should appear in your output folder.

#### Using Adjectivest in Code
______________

Add **'Adjectivest'** to your using statements in whatever class you're using Adjectivest with.

The only class you should need to utilize in a typical scenario is the **AdjectiveInflector** class, and you should only need one instance.

##### Loading the resources into memory
______________

The only parameter in the class's constructor is the 'loadDictionaryIntoMemory' bool.  This determines whether the CMU Dictionary is loaded into memory or accessed on-demand.  The difference may be obvious, but loading it into memory is faster on each individual request at the cost of, well, memory.  Accessing it on each request is a bit slower but more memory-efficient.  While this of course depends on the specifications of both the drive and memory in the environment on which the code is deployed, you can expect the in-memory option to be ~10-15x faster than the file-read option.  I'd recommend the in-memory option unless your deployment platform is really hurting for memory.

`AdjectiveInflector inflector = new AdjectiveInflector(true);`

#### Examples:
______________

Use the **'GetAdjectiveInflections'** method to get a pipe-delimited string of the adjective's base, comparative, and superlative forms (in that order).

*Entering a base, comparative, or superlative form of the same adjective should get the same result.*

```
string adj = "heavy";

string inflection = inflector.GetAdjectiveInflections(adj);

//output: "heavy|heavier|heaviest"
//an input of "heavier" or "heaviest" would have same output.
```

((VB.Net 2010 conversion 24/12/2021 by Roger Pearse))
