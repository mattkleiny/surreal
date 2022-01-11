namespace Surreal.Utilities;

public class TransactionTests
{
  [Test, AutoFixture]
  public async Task it_should_create_and_commit_a_valid_transaction(ITransactionOperation operation1, ITransactionOperation operation2, ITransactionOperation operation3)
  {
    using var transaction = Transaction.Create();

    transaction.Push(operation1);
    transaction.Push(operation2);
    transaction.Push(operation3);

    await transaction.CommitAsync();

    await operation1.Received(1).CommitAsync();
    await operation2.Received(1).CommitAsync();
    await operation3.Received(1).CommitAsync();

    await transaction.RollbackAsync();

    await operation1.Received(1).RollbackAsync();
    await operation2.Received(1).RollbackAsync();
    await operation3.Received(1).RollbackAsync();
  }
}
