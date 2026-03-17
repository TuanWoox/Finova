---
name: finova-api-service-pattern
description: "Implement consistent API service methods and controllers for Finova. Use when: creating new controllers, writing service methods, building CRUD APIs, ensuring uniform error handling, following ReturnResult pattern, and maintaining code consistency across API endpoints."
---

# Finova CRUD Controller + Service Skill

Use this skill whenever you add or update CRUD APIs in Finova.

This is the default standard for:
- controllers
- service methods
- repository usage
- response format
- auth/ownership checks

## Fixed Conventions

1. Always return `ReturnResult<T>` from service methods.
2. Controllers always return `Ok(returnResult)`.
3. Controllers are thin; business rules stay in services.
4. Every method is `try-catch` and logs with `AppLogger.Instance.Debug(ex.Message)`.
5. Resource ownership is validated in service before update/delete/delete-many.
6. Duplicate account name is blocked on create and update.

## Layer Responsibilities

Controller layer:
- map HTTP endpoint to service call
- initialize typed `ReturnResult<T>`
- catch unexpected exceptions

Service layer:
- input validation
- authentication check (`_userContext.UserId`)
- ownership check
- duplicate-name check
- repository calls and mapping

Repository layer:
- generic persistence operations (`CreateAsync`, `GetByIdAsync`, `UpdateAsync`, `DeleteByIdAsync`, `DeleteByIdsAsync`)
- no user-specific ownership logic

## ReturnResult Rules

Success:
```json
{
  "result": { },
  "message": null
}
```

Failure:
```json
{
  "result": null,
  "message": "Error message"
}
```

Rule:
- set `Result` on success
- set `Message` on failure

## Controller Template

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResourceController : ControllerBase
{
    private readonly IResourceService _resourceService;

    public ResourceController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateResourceDTO dto)
    {
        ReturnResult<ResourceDTO> returnResult = new ReturnResult<ResourceDTO>();
        try
        {
            returnResult = await _resourceService.CreateAsync(dto);
        }
        catch (Exception ex)
        {
            AppLogger.Instance.Debug(ex.Message);
            returnResult.Message = $"An error occurred: {ex.Message}";
        }
        return Ok(returnResult);
    }
}
```

## Service Method Template (Single Resource)

```csharp
public async Task<ReturnResult<ResourceDTO>> UpdateAsync(UpdateResourceDTO updateDTO)
{
    ReturnResult<ResourceDTO> returnResult = new ReturnResult<ResourceDTO>();
    try
    {
        // Step 1: validate input
        if (string.IsNullOrEmpty(updateDTO.Id))
        {
            returnResult.Message = "Invalid resource ID";
            return returnResult;
        }

        // Step 2: validate auth
        if (string.IsNullOrEmpty(_userContext.UserId))
        {
            returnResult.Message = "User not authenticated";
            return returnResult;
        }

        // Step 3: load and verify ownership
        var getResult = await _resourceRepository.GetByIdAsync(updateDTO.Id);
        if (getResult.Result == null)
        {
            returnResult.Message = $"Resource {updateDTO.Id} not found";
            return returnResult;
        }

        if (getResult.Result.OwnerId != _userContext.UserId)
        {
            returnResult.Message = "Access denied: You do not own this resource";
            return returnResult;
        }

        // Step 4: execute update
        var result = await _resourceRepository.UpdateAsync<UpdateResourceDTO>(updateDTO);
        if (result.Result != null)
        {
            returnResult.Result = _mapper.Map<ResourceDTO>(result.Result);
        }
        else
        {
            returnResult.Message = result.Message;
        }
    }
    catch (Exception ex)
    {
        AppLogger.Instance.Debug(ex.Message);
        returnResult.Message = $"An error occurred while updating resource: {ex.Message}";
    }
    return returnResult;
}
```

## Account-Specific Rules (Current Project Standard)

Create account:
- require authenticated user
- require non-empty `Name`
- reject duplicate name for same owner
- set `OwnerId = _userContext.UserId`

Update account:
- require authenticated user
- require valid `Id`
- account must exist and belong to current user
- reject duplicate `Name` for same owner excluding current account (`a.Id != updateAccountDTO.Id`)

Delete one:
- require authenticated user
- account must exist and belong to current user
- call `DeleteByIdAsync(accountId)`

Delete many:
- require authenticated user
- require non-empty `List<string> ids`
- verify ownership count equals requested count
- pass full `ids` list directly to repository `DeleteByIdsAsync(ids)`

## Bulk Delete Pattern (Required)

Repository method `DeleteByIdsAsync(List<TKey> ids)` deletes by ids only.
Because it does not enforce ownership, service must enforce ownership first.

Use this exact flow:

```csharp
public async Task<ReturnResult<int>> DeletAccountsAsync(List<string> ids)
{
    ReturnResult<int> returnResult = new ReturnResult<int>();
    try
    {
        if (ids == null || ids.Count == 0)
        {
            returnResult.Message = "Invalid account IDs";
            return returnResult;
        }

        if (string.IsNullOrEmpty(_userContext.UserId))
        {
            returnResult.Message = "User not authenticated";
            return returnResult;
        }

        var ownedAccountsCount = await _dbContext.Accounts
            .Where(a => ids.Contains(a.Id) && a.OwnerId == _userContext.UserId)
            .CountAsync();

        if (ownedAccountsCount != ids.Count)
        {
            returnResult.Message = "Access denied: You do not own all requested accounts";
            return returnResult;
        }

        var result = await _accountRepository.DeleteByIdsAsync(ids);
        returnResult.Result = result.Result;

        if (result.Result == 0)
        {
            returnResult.Message = result.Message;
        }
    }
    catch (Exception ex)
    {
        AppLogger.Instance.Debug(ex.Message);
        returnResult.Message = $"An error occurred while deleting accounts: {ex.Message}";
    }
    return returnResult;
}
```

## CRUD Endpoint Set (Account Pattern)

Use these endpoint conventions:
- `POST create`
- `GET {accountId}`
- `POST my-accounts`
- `PUT update`
- `DELETE {accountId}`
- `POST deletemany`

## Type Matching Rules

Controller return variable generic type must match service return type exactly.

Examples:
- `CreateAccountAsync` -> `ReturnResult<AccountDTO>`
- `GetMyAccountsAsync` -> `ReturnResult<PagedData<AccountDTO, string>>`
- `DeleteAccountAsync` -> `ReturnResult<bool>`
- `DeletAccountsAsync` -> `ReturnResult<int>`

Never use `ReturnResult<object>` if a concrete type exists.

## Error Message Style

Use consistent messages:
- "User not authenticated"
- "Invalid account ID"
- "Invalid account name"
- "Account {id} not found"
- "Access denied: You do not own this account"
- "Account with name '{name}' already exists"

## Quick Checklist Before Finishing a New CRUD API

- Interface updated in Common project
- Service implementation added in Business project
- Controller endpoints added in API project
- DI registration added
- Correct typed `ReturnResult<T>` used everywhere
- Duplicate-name rule implemented when needed
- Ownership check implemented for read/update/delete/delete-many
- Delete-many validates ownership before repository call

### Example 2: READ Operation

**Service Method**
```csharp
public async Task<ReturnResult<AccountDTO>> GetAccountByIdAsync(string accountId)
{
    ReturnResult<AccountDTO> returnResult = new ReturnResult<AccountDTO>();
    try
    {
        // Step 1: Validate input
        if (string.IsNullOrEmpty(accountId))
        {
            returnResult.Message = "Invalid account ID";
            return returnResult;
        }

        // Step 2: Check authentication
        if (string.IsNullOrEmpty(_userContext.UserId))
        {
            returnResult.Message = "User not authenticated";
            return returnResult;
        }

        // Step 3: Get resource
        var result = await _accountRepository.GetByIdAsync(accountId);
        if (result.Result == null)
        {
            returnResult.Message = $"Account {accountId} not found";
            return returnResult;
        }

        // Step 4: Verify ownership
        if (result.Result.OwnerId != _userContext.UserId)
        {
            returnResult.Message = "Access denied: You do not own this account";
            return returnResult;
        }

        // Step 5: Map and return
        var accountDTO = _mapper.Map<AccountDTO>(result.Result);
        accountDTO.OwnerName = _userContext.UserName;
        returnResult.Result = accountDTO;
    }
    catch (Exception ex)
    {
        AppLogger.Instance.Debug(ex.Message);
        returnResult.Message = $"An error occurred while retrieving account: {ex.Message}";
    }
    return returnResult;
}
```

**Controller Method**
```csharp
[HttpGet("{accountId}")]
public async Task<IActionResult> GetAccountById(string accountId)
{
    ReturnResult<AccountDTO> result = new ReturnResult<AccountDTO>();
    try
    {
        result = await _accountService.GetAccountByIdAsync(accountId);
    }
    catch (Exception ex)
    {
        AppLogger.Instance.Debug(ex.Message);
        result.Message = $"An error occurred: {ex.Message}";
    }
    return Ok(result);
}
```

### Example 3: UPDATE Operation

**Service Method**
```csharp
public async Task<ReturnResult<AccountDTO>> UpdateAccountAsync(UpdateAccountDTO dto)
{
    ReturnResult<AccountDTO> returnResult = new ReturnResult<AccountDTO>();
    try
    {
        // Step 1: Validate input
        if (string.IsNullOrEmpty(dto.Id))
        {
            returnResult.Message = "Invalid account ID";
            return returnResult;
        }

        // Step 2: Check authentication
        if (string.IsNullOrEmpty(_userContext.UserId))
        {
            returnResult.Message = "User not authenticated";
            return returnResult;
        }

        // Step 3: Get existing resource
        var getResult = await _accountRepository.GetByIdAsync(dto.Id);
        if (getResult.Result == null)
        {
            returnResult.Message = $"Account {dto.Id} not found";
            return returnResult;
        }

        // Step 4: Verify ownership
        if (getResult.Result.OwnerId != _userContext.UserId)
        {
            returnResult.Message = "Access denied: You do not own this account";
            return returnResult;
        }

        // Step 5: Update
        var result = await _accountRepository.UpdateAsync<UpdateAccountDTO>(dto);
        if (result.Result == null)
        {
            returnResult.Message = result.Message;
            return returnResult;
        }

        // Step 6: Map and return
        var accountDTO = _mapper.Map<AccountDTO>(result.Result);
        accountDTO.OwnerName = _userContext.UserName;
        returnResult.Result = accountDTO;
    }
    catch (Exception ex)
    {
        AppLogger.Instance.Debug(ex.Message);
        returnResult.Message = $"An error occurred while updating account: {ex.Message}";
    }
    return returnResult;
}
```

**Controller Method**
```csharp
[HttpPut("update")]
public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountDTO dto)
{
    ReturnResult<AccountDTO> result = new ReturnResult<AccountDTO>();
    try
    {
        result = await _accountService.UpdateAccountAsync(dto);
    }
    catch (Exception ex)
    {
        AppLogger.Instance.Debug(ex.Message);
        result.Message = $"An error occurred: {ex.Message}";
    }
    return Ok(result);
}
```

### Example 4: DELETE Operation

**Service Method**
```csharp
public async Task<ReturnResult<bool>> DeleteAccountAsync(string accountId)
{
    ReturnResult<bool> returnResult = new ReturnResult<bool>();
    try
    {
        // Step 1: Validate input
        if (string.IsNullOrEmpty(accountId))
        {
            returnResult.Message = "Invalid account ID";
            return returnResult;
        }

        // Step 2: Check authentication
        if (string.IsNullOrEmpty(_userContext.UserId))
        {
            returnResult.Message = "User not authenticated";
            return returnResult;
        }

        // Step 3: Get existing resource
        var getResult = await _accountRepository.GetByIdAsync(accountId);
        if (getResult.Result == null)
        {
            returnResult.Message = $"Account {accountId} not found";
            return returnResult;
        }

        // Step 4: Verify ownership
        if (getResult.Result.OwnerId != _userContext.UserId)
        {
            returnResult.Message = "Access denied: You do not own this account";
            return returnResult;
        }

        // Step 5: Delete
        var result = await _accountRepository.DeleteByIdAsync(accountId);
        returnResult.Result = result.Result;

        if (!result.Result)
        {
            returnResult.Message = result.Message;
        }
    }
    catch (Exception ex)
    {
        AppLogger.Instance.Debug(ex.Message);
        returnResult.Message = $"An error occurred while deleting account: {ex.Message}";
    }
    return returnResult;
}
```

**Controller Method**
```csharp
[HttpDelete("{accountId}")]
public async Task<IActionResult> DeleteAccount(string accountId)
{
    ReturnResult<bool> result = new ReturnResult<bool>();
    try
    {
        result = await _accountService.DeleteAccountAsync(accountId);
    }
    catch (Exception ex)
    {
        AppLogger.Instance.Debug(ex.Message);
        result.Message = $"An error occurred: {ex.Message}";
    }
    return Ok(result);
}
```

### Example 5: LIST with Pagination

**Service Method**
```csharp
public async Task<ReturnResult<PagedData<AccountDTO, string>>> GetMyAccountsAsync(Page<string> page)
{
    ReturnResult<PagedData<AccountDTO, string>> returnResult = 
        new ReturnResult<PagedData<AccountDTO, string>>();
    try
    {
        // Step 1: Check authentication
        if (string.IsNullOrEmpty(_userContext.UserId))
        {
            returnResult.Message = "User not authenticated";
            return returnResult;
        }

        // Step 2: Validate pagination
        if (page == null)
        {
            returnResult.Message = "Invalid page parameters";
            return returnResult;
        }

        // Step 3: Create filtered query for current user
        var query = _dbContext.Accounts
            .Where(a => a.OwnerId == _userContext.UserId)
            .AsQueryable();

        // Step 4: Get paginated data
        var pagedData = await _accountRepository
            .GetPagingAsync<Page<string>, AccountDTO>(query, page);

        // Step 5: Enrich with owner name
        if (pagedData.Data != null)
        {
            foreach (var accountDTO in pagedData.Data)
            {
                accountDTO.OwnerName = _userContext.UserName;
            }
        }

        returnResult.Result = pagedData;
    }
    catch (Exception ex)
    {
        AppLogger.Instance.Debug(ex.Message);
        returnResult.Message = $"An error occurred while retrieving accounts: {ex.Message}";
    }
    return returnResult;
}
```

**Controller Method**
```csharp
[HttpPost("my-accounts")]
public async Task<IActionResult> GetMyAccounts([FromBody] Page<string> page)
{
    ReturnResult<PagedData<AccountDTO, string>> result = 
        new ReturnResult<PagedData<AccountDTO, string>>();
    try
    {
        result = await _accountService.GetMyAccountsAsync(page);
    }
    catch (Exception ex)
    {
        AppLogger.Instance.Debug(ex.Message);
        result.Message = $"An error occurred: {ex.Message}";
    }
    return Ok(result);
}
```

---

## Common Validation Patterns

### Authentication Check
```csharp
if (string.IsNullOrEmpty(_userContext.UserId))
{
    returnResult.Message = "User not authenticated";
    return returnResult;
}
```

### Ownership Verification
```csharp
if (resource.OwnerId != _userContext.UserId)
{
    returnResult.Message = "Access denied: You do not own this resource";
    return returnResult;
}
```

### Null/Empty Validation
```csharp
if (string.IsNullOrWhiteSpace(dto.Name))
{
    returnResult.Message = "Invalid {field name}";
    return returnResult;
}
```

### Duplicate Check
```csharp
var existing = await _repository.FirstOrDefaultAsync(x => x.Name == dto.Name);
if (existing != null)
{
    returnResult.Message = "Resource with this name already exists";
    return returnResult;
}
```

### Resource Not Found
```csharp
var resource = await _repository.GetByIdAsync(id);
if (resource == null)
{
    returnResult.Message = "Resource not found";
    return returnResult;
}
```

---

## Error Message Format Guide

Follow these conventions for consistent error messages:

| Scenario | Pattern | Example |
|----------|---------|---------|
| Duplicate | `"{Resource} with this {field} already exists"` | "Account with this name already exists" |
| Not Found | `"{Resource} {identifier} not found"` | "Account ABC123 not found" |
| Invalid Data | `"Invalid {field}: {reason}"` | "Invalid email: Must be a valid email address" |
| Access Denied | `"Access denied: {reason}"` | "Access denied: You do not own this account" |
| Not Authenticated | `"User not authenticated"` | "User not authenticated" |
| General Error | `"An error occurred while {action}: {message}"` | "An error occurred while creating account: ..." |

---

## Best Practices

### DO ✓

- ✓ Initialize `ReturnResult<T>` with correct generic type
- ✓ Validate inputs as early as possible (Step 1)
- ✓ Check authentication before accessing user resources
- ✓ Verify ownership before CRUD operations on user data
- ✓ Map entities to DTOs before returning
- ✓ Log all exceptions with `AppLogger.Instance.Debug()`
- ✓ Use descriptive, actionable error messages
- ✓ Return `Ok(result)` always (let client check `message` field)
- ✓ Keep controllers thin - all logic in services
- ✓ Use the same validation order across all similar methods

### DON'T ✗

- ✗ Return different HTTP status codes (always use 200 Ok)
- ✗ Mix business logic in controllers
- ✗ Skip exception handling in controllers
- ✗ Return entities directly (always map to DTOs)
- ✗ Set both `Result` and `Message` in the same response
- ✗ Perform validation in controllers
- ✗ Skip ownership verification for user resources
- ✗ Return without logging exceptions
- ✗ Create `ReturnResult` inside try block
- ✗ Use different error message formats

---

## Common Gotchas

### Gotcha 1: Wrong Generic Type
```csharp
// ✗ WRONG - Type mismatch
ReturnResult<object> result = new ReturnResult<object>();
result = await _service.GetAccountAsync(); // Returns ReturnResult<AccountDTO>

// ✓ CORRECT - Types match
ReturnResult<AccountDTO> result = new ReturnResult<AccountDTO>();
result = await _service.GetAccountAsync(); // Returns ReturnResult<AccountDTO>
```

### Gotcha 2: Setting Both Result and Message
```csharp
// ✗ WRONG - Confusing response
returnResult.Result = accountDTO;
returnResult.Message = "Success"; // Should be null

// ✓ CORRECT - Only one is set
if (success)
{
    returnResult.Result = accountDTO; // Message stays null
}
else
{
    returnResult.Message = "Error message"; // Result stays null
}
```

### Gotcha 3: Forgetting to Check Ownership
```csharp
// ✗ WRONG - Any authenticated user can access any account
var account = await _repo.GetByIdAsync(id);
return account;

// ✓ CORRECT - Verify ownership first
var account = await _repo.GetByIdAsync(id);
if (account.OwnerId != _userContext.UserId)
{
    returnResult.Message = "Access denied";
    return returnResult;
}
returnResult.Result = account;
```

### Gotcha 4: Not Initializing ReturnResult in Controller
```csharp
// ✗ WRONG - Null reference if service throws before assignment
async Task<IActionResult> Create()
{
    try
    {
        return await _service.CreateAsync(); // No initialization!
    }
    catch (Ex ex) { /* Can't set message */ }
}

// ✓ CORRECT - Always initialize
async Task<IActionResult> Create()
{
    ReturnResult<ResourceDTO> result = new ReturnResult<ResourceDTO>(); // Initialize first
    try
    {
        result = await _service.CreateAsync();
    }
    catch (Exception ex)
    {
        result.Message = $"Error: {ex.Message}";
    }
    return Ok(result);
}
```

---

## Quick Checklist for New CRUD Operations

When implementing a new CRUD resource:

### Service Implementation
- [ ] Create interface in `core-backend.Common/Interfaces/Services/I{Resource}Service.cs`
- [ ] Implement service in `core-backend.Business/Services/{Resource}Service.cs`
- [ ] Follow all 5 steps in each method (Validate → Check Auth → Verify Ownership → Execute → Return)
- [ ] Add proper error messages for each validation
- [ ] Map entities to DTOs before returning
- [ ] Register in `ServiceExtensions.cs`

### Controller Implementation
- [ ] Create controller in `core-backend/Controllers/{Resource}Controller.cs`
- [ ] Add `[Authorize]` attribute
- [ ] Create all 5 methods: Create, Get, List, Update, Delete
- [ ] Wrap each call in try-catch
- [ ] Initialize `ReturnResult<T>` with correct type
- [ ] Add XML documentation above each method
- [ ] Use consistent routing pattern (`create`, `list`, etc.)

### Testing Checklist
- [ ] Test unauthorized access (no token)
- [ ] Test resource not found (404 equivalent)
- [ ] Test access to other user's data (ownership)
- [ ] Test invalid input validation
- [ ] Test successful CRUD operations
- [ ] Verify error messages are descriptive
- [ ] Verify response structure matches pattern

---

## Service Registration Example

In `Infrastructures/Service/ServiceExtensions.cs`:

```csharp
public static void AddApplicationServices(this IServiceCollection services)
{
    // Register services
    services.AddScoped<IAccountService, AccountService>();
    services.AddScoped<IAuthService, AuthService>();
    // Add more services here as you create them
}
```

---

## Real-World Example

See [AccountController.cs](../../Controllers/AccountController.cs) and [AccountService.cs](../../Business/Services/AccountService.cs) for complete working examples of this pattern.
```csharp
public async Task<ReturnResult<Account>> UpdateAccount(string id, UpdateAccountDTO dto)
{
    ReturnResult<Account> returnResult = new ReturnResult<Account>();
    try
    {
        // Find
        var account = await _context.Accounts.FindAsync(id);
        if (account == null)
        {
            returnResult.Message = $"Account {id} not found";
            return returnResult;
        }

        // Update
        account.Name = dto.Name;
        account.DateModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        returnResult.Result = account;
    }
    catch (Exception ex)
    {
        AppLogger.Instance.Debug(ex.Message);
        returnResult.Message = $"An error occurred while updating account: {ex.Message}";
    }
    return returnResult;
}
```

### Delete Operation
```csharp
public async Task<ReturnResult<bool>> DeleteAccount(string id)
{
    ReturnResult<bool> returnResult = new ReturnResult<bool>();
    try
    {
        // Find
        var account = await _context.Accounts.FindAsync(id);
        if (account == null)
        {
            returnResult.Message = $"Account {id} not found";
            return returnResult;
        }

        // Soft delete
        account.Deleted = true;
        account.DateDeleted = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();

        returnResult.Result = true;
    }
    catch (Exception ex)
    {
        AppLogger.Instance.Debug(ex.Message);
        returnResult.Message = $"An error occurred while deleting account: {ex.Message}";
    }
    return returnResult;
}
```

### Get/Query Operation
```csharp
public async Task<ReturnResult<List<Account>>> GetUserAccounts(string userId)
{
    ReturnResult<List<Account>> returnResult = new ReturnResult<List<Account>>();
    try
    {
        // Validate user exists
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            returnResult.Message = $"User {userId} not found";
            return returnResult;
        }

        // Query
        var accounts = await _context.Accounts
            .Where(a => a.OwnerId == userId && !a.Deleted)
            .ToListAsync();

        returnResult.Result = accounts;
    }
    catch (Exception ex)
    {
        AppLogger.Instance.Debug(ex.Message);
        returnResult.Message = $"An error occurred while retrieving accounts: {ex.Message}";
    }
    return returnResult;
}
```

## Checklist for New Service Methods

- [ ] Method returns `ReturnResult<T>`
- [ ] Initializes `ReturnResult<T> returnResult = new ReturnResult<T>();`
- [ ] Wrapped in try-catch block
- [ ] Validates inputs before business logic
- [ ] Returns early on validation failure with only `Message` set
- [ ] Returns success with only `Result` set (no message)
- [ ] Catches exceptions and logs via AppLogger
- [ ] Returns returnResult at end
- [ ] Error messages are descriptive and consistent

## Best Practices

1. **Validate Early**: Check constraints before any database operations
2. **Fail Fast**: Return immediately on validation failure
3. **Log Everything**: Use AppLogger.Instance.Debug() for debugging
4. **No Partial Results**: Either full success (Result set) or full failure (Message set), never both
5. **Consistent Naming**: Use async/await, follow naming conventions
6. **DTOs**: Use separate DTOs for input, output, and updates
7. **Business Rules**: Keep validation logic in service, not in controllers

## Utility Components for Easier Development

### 1. Generic Repository Pattern

The `Repository<TEntity, TKey>` class provides common CRUD operations with automatic mapping:

```csharp
public class MyService
{
    private readonly IRepository<Account, string> _accountRepository;

    public MyService(IRepository<Account, string> accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<ReturnResult<Account>> CreateAccountAsync(CreateAccountDTO dto)
    {
        return await _accountRepository.CreateAsync<CreateAccountDTO>(dto);
    }

    public async Task<ReturnResult<Account>> GetAccountAsync(string id)
    {
        return await _accountRepository.GetByIdAsync(id);
    }

    public async Task<ReturnResult<Account>> UpdateAccountAsync(UpdateAccountDTO dto)
    {
        return await _accountRepository.UpdateAsync<UpdateAccountDTO>(dto);
    }

    public async Task<ReturnResult<bool>> DeleteAccountAsync(string id)
    {
        return await _accountRepository.DeleteByIdAsync(id);
    }
}
```

**Repository Methods Available:**
- `CreateAsync<TCreateDto>(entity)` - Create new entity
- `GetByIdAsync(id)` - Get single entity
- `UpdateAsync<TUpdateDto>(entity)` - Update entity
- `DeleteByIdAsync(id)` - Delete single entity
- `DeleteByIdsAsync(ids)` - Delete multiple entities
- `GetPagingAsync<TPage, TResponse>(entities, page)` - Get paginated results

**Important**: The repository already returns `ReturnResult<T>`, so error handling is built-in.

### 2. UserContext - Current User Access

Inject `IUserContext` to get current authenticated user's information:

```csharp
public class AccountService : IAccountService
{
    private readonly IRepository<Account, string> _accountRepository;
    private readonly IUserContext _userContext;

    public AccountService(
        IRepository<Account, string> accountRepository,
        IUserContext userContext
    )
    {
        _accountRepository = accountRepository;
        _userContext = userContext;
    }

    public async Task<ReturnResult<List<Account>>> GetMyAccountsAsync()
    {
        ReturnResult<List<Account>> returnResult = new ReturnResult<List<Account>>();
        try
        {
            // Get current user ID from context
            var userId = _userContext.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                returnResult.Message = "User not authenticated";
                return returnResult;
            }

            // Query user's accounts
            var accounts = await _accountRepository.GetMyAccountsAsync(userId);
            returnResult.Result = accounts;
        }
        catch (Exception ex)
        {
            AppLogger.Instance.Debug(ex.Message);
            returnResult.Message = $"An error occurred: {ex.Message}";
        }
        return returnResult;
    }

    public async Task<ReturnResult<Account>> AdminCreateAccountAsync(CreateAccountDTO dto)
    {
        ReturnResult<Account> returnResult = new ReturnResult<Account>();
        try
        {
            // Check if current user is admin
            if (!_userContext.IsAdmin)
            {
                returnResult.Message = "Only admins can create accounts for other users";
                return returnResult;
            }

            return await _accountRepository.CreateAsync<CreateAccountDTO>(dto);
        }
        catch (Exception ex)
        {
            AppLogger.Instance.Debug(ex.Message);
            returnResult.Message = $"An error occurred: {ex.Message}";
        }
        return returnResult;
    }
}
```

**UserContext Properties:**
- `UserId` - Current user's ID (from JWT claim)
- `IsAdmin` - Check if user has Admin role
- `UserName` - Current user's username
- `Email` - Current user's email address

### 3. Complete Service Example Using Both

```csharp
public class TransactionService : ITransactionService
{
    private readonly IRepository<Transaction, string> _transactionRepository;
    private readonly IUserContext _userContext;

    public TransactionService(
        IRepository<Transaction, string> transactionRepository,
        IUserContext userContext
    )
    {
        _transactionRepository = transactionRepository;
        _userContext = userContext;
    }

    public async Task<ReturnResult<Transaction>> RecordTransactionAsync(CreateTransactionDTO dto)
    {
        ReturnResult<Transaction> returnResult = new ReturnResult<Transaction>();
        try
        {
            // Step 1: Validate user is authenticated
            if (string.IsNullOrEmpty(_userContext.UserId))
            {
                returnResult.Message = "User not authenticated";
                return returnResult;
            }

            // Step 2: Validate input
            if (dto.Amount <= 0)
            {
                returnResult.Message = "Invalid amount: must be greater than zero";
                return returnResult;
            }

            // Step 3: Set owner to current user
            dto.OwnerId = _userContext.UserId;

            // Step 4: Create transaction via repository
            var result = await _transactionRepository.CreateAsync<CreateTransactionDTO>(dto);
            
            return result;
        }
        catch (Exception ex)
        {
            AppLogger.Instance.Debug(ex.Message);
            returnResult.Message = $"An error occurred while recording transaction: {ex.Message}";
        }
        return returnResult;
    }

    public async Task<ReturnResult<List<Transaction>>> GetMyTransactionsAsync()
    {
        ReturnResult<List<Transaction>> returnResult = new ReturnResult<List<Transaction>>();
        try
        {
            // Get current user's ID
            var userId = _userContext.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                returnResult.Message = "User not authenticated";
                return returnResult;
            }

            // Query transactions for current user
            var transactions = await _transactionRepository
                .GetMyTransactionsAsync(userId);

            returnResult.Result = transactions;
        }
        catch (Exception ex)
        {
            AppLogger.Instance.Debug(ex.Message);
            returnResult.Message = $"An error occurred while retrieving transactions: {ex.Message}";
        }
        return returnResult;
    }
}
```

## Service Method Registration

Register your services in `RegisterDI.cs`:

```csharp
public static IServiceCollection RegisterServiceDI(this IServiceCollection services)
{
    services.TryAddScoped(typeof(IRepository<,>), typeof(Repository<,>));
    services.TryAddScoped<IUserContext, UserContext>();
    services.TryAddScoped<IAuthService, AuthService>();
    services.TryAddScoped<IAccountService, AccountService>();
    services.TryAddScoped<ITransactionService, TransactionService>();
    return services;
}
```

## Mapping Configuration

Ensure AutoMapper is configured with your DTOs:

```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Auth
        CreateMap<RegisterAccountDTO, ApplicationUser>();
        
        // Accounts
        CreateMap<CreateAccountDTO, Account>();
        CreateMap<UpdateAccountDTO, Account>();
        CreateMap<Account, AccountDTO>();
        
        // Transactions
        CreateMap<CreateTransactionDTO, Transaction>();
        CreateMap<UpdateTransactionDTO, Transaction>();
        CreateMap<Transaction, TransactionDTO>();
    }
}
```

## Complete Workflow Pattern

```csharp
public async Task<ReturnResult<Transaction>> RecordTransactionAsync(CreateTransactionDTO dto)
{
    ReturnResult<Transaction> returnResult = new ReturnResult<Transaction>();
    try
    {
        // 1. Validate user context
        if (string.IsNullOrEmpty(_userContext.UserId))
        {
            returnResult.Message = "User not authenticated";
            return returnResult;
        }

        // 2. Validate inputs
        if (dto.Amount <= 0)
        {
            returnResult.Message = "Invalid amount";
            return returnResult;
        }

        // 3. Set owner to current user
        dto.OwnerId = _userContext.UserId;

        // 4. Use repository (already returns ReturnResult)
        var result = await _transactionRepository.CreateAsync<CreateTransactionDTO>(dto);
        
        // 5. Return repository result directly
        return result;
    }
    catch (Exception ex)
    {
        AppLogger.Instance.Debug(ex.Message);
        returnResult.Message = $"An error occurred during transaction: {ex.Message}";
    }
    return returnResult;
}
```

