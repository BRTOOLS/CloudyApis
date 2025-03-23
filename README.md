Need to remove this part




public static Task<string> AskAi(string input)
		{
			Api.<AskAi>d__14 <AskAi>d__ = new Api.<AskAi>d__14();
			<AskAi>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<AskAi>d__.input = input;
			<AskAi>d__.<>1__state = -1;
			<AskAi>d__.<>t__builder.Start<Api.<AskAi>d__14>(ref <AskAi>d__);
			return <AskAi>d__.<>t__builder.Task;
		}



