namespace PaymentServices.Commands.Contracts;

public interface ICommand
{
    void Execute();

    bool IsExecutable();

    void Undo();
}