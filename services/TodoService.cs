using Microsoft.EntityFrameworkCore;
using TodoApi;

public class TodoService
{
    private readonly ToDoDbContext _todoDbContext;

    public TodoService(ToDoDbContext todoDbContext)
    {
        _todoDbContext = todoDbContext;
    }

    // Get all items
    public async Task<IEnumerable<Item>> GetItems()
    {
        try
        {
            var items = await _todoDbContext.Items.ToListAsync();

            // No need to check for null, since ToListAsync() will return an empty list
            return items.Any() ? items : Enumerable.Empty<Item>();
        }
        catch (Exception ex)
        {
            // Log exception here if necessary
            Console.WriteLine($"Error retrieving items: {ex.Message}"); // Log the exception
            throw;
        }
    }

    //     public IEnumerable<Item> GetItems()
    // {
    //     var items = _todoDbContext.Items.ToList(); // גרסה סינכרונית לצורך דיבוג
    //     return items.Any() ? items : Enumerable.Empty<Item>();
    // }
    // Create a new item
    public async Task<string> PostItem(Item item)
    {
        try
        {
            if (item == null)
                return "Item is null. Can't create item.";

            await _todoDbContext.AddAsync(item);
            await _todoDbContext.SaveChangesAsync();

            return "New item created successfully.";
        }
        catch (Exception ex)
        {
            // Log exception here if necessary
            throw new Exception("Error creating item", ex);
        }
    }

    // Delete an item by id
    public async Task<string> DeleteItem(int id)
    {
        try
        {
            var item = await _todoDbContext.Items.FindAsync(id);
            if (item == null)
                return "Item not found.";

            _todoDbContext.Remove(item);
            await _todoDbContext.SaveChangesAsync();

            return "Item deleted successfully.";
        }
        catch (Exception ex)
        {
            // Log exception here if necessary
            throw new Exception("Error deleting item", ex);
        }
    }

    // Update an existing item
    public async Task<string> PutItem(int id, bool? isComplete)
    {
        try
        {
            var item = await _todoDbContext.Items.FindAsync(id);
            if (item == null)
                return "Item not found.";

            item.IsComplete = isComplete;

            await _todoDbContext.SaveChangesAsync();

            return "Item updated successfully.";
        }
        catch (Exception ex)
        {
            // Log exception here if necessary
            throw new Exception("Error updating item", ex);
        }
    }
}
