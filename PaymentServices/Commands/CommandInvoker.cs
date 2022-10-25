using PaymentServices.Commands.Contracts;

namespace PaymentServices.Commands;

public class CommandInvoker
{
    private readonly Stack<ICommand> commands;

    public CommandInvoker()
    {
        this.commands = new();
    }

    public bool Invoke(ICommand command)
    {
        bool isSuccessful = false;
        
        if (command.IsExecutable())
        {
            command.Execute();
            commands.Push(command);
            isSuccessful = true;
        }

        return isSuccessful;
    }
    
    public void Undo()
    {
        ICommand command = commands.Pop();
        command.Undo();
    }
}