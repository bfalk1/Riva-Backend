# Assignment: Credit Enforcement Engine

## Overview

You are tasked with building a high-performance credit enforcement system for an API. The system must:
- Deduct 1 credit per API request
- Block users with 0 credits (return `402 Payment Required`)
- Handle concurrent requests without race conditions
- Meet a strict **20ms latency requirement**

## The Challenge

The existing `LegacyBillingRepository` simulates a slow database with **100ms latency per operation**. You cannot call it on every request. You must implement a caching layer that:
- Keeps response times under 20ms
- Prevents double-spending (race conditions)
- Ensures only one request succeeds when a user has exactly 1 credit remaining

## What You Need to Implement

### 1. CreditService (`RivaAssessment/Services/CreditService.cs`)

**Current Status:** Throws `NotImplementedException`

**Your Task:**
- Implement `TryDeductCreditAsync(string userId)` method
- Add caching to avoid calling `LegacyBillingRepository` on every request
- Implement concurrency control (locking/atomic operations) to prevent race conditions
- Ensure the operation completes in **< 20ms**

**Key Requirements:**
- Cache credit balances in memory
- Use atomic operations or per-user locking
- Handle the case where 10 concurrent requests arrive for a user with 1 credit (only 1 should succeed)
- Return `true` if credit was deducted, `false` if insufficient credits

**Suggested Approach:**
- Use `ConcurrentDictionary` for thread-safe caching
- Use `SemaphoreSlim` for per-user locking
- Implement cache-aside pattern: check cache first, load from repository if needed
- Update cache atomically when deducting credits

### 2. CreditEnforcementMiddleware (`RivaAssessment/Middleware/CreditEnforcementMiddleware.cs`)

**Current Status:** Partially implemented, needs completion

**Your Task:**
- Complete the middleware implementation
- Extract user ID from `X-User-Id` header (already done)
- Call `CreditService.TryDeductCreditAsync()` 
- Return `402 Payment Required` if user has no credits
- Allow request to proceed if credit was successfully deducted

**Registration:**
- Uncomment the middleware registration in `Program.cs`:
  ```csharp
  app.UseMiddleware<CreditEnforcementMiddleware>();
  ```

### 3. Tests (`RivaAssessment.Tests/CreditServiceTests.cs`)

**Current Status:** Test structure exists, but needs to be completed and verified

**Your Task:**
- Complete the test implementations
- Fix any test setup issues (e.g., mock repository behavior)
- Ensure the concurrency test correctly creates 10 concurrent tasks
- Verify all assertions are correct
- Ensure all tests pass once CreditService is implemented

**Test 1: `TryDeductCredit_ReturnsFalse_WhenUserHasNoCredits`**
- This test verifies that users with 0 credits cannot deduct credits
- Mock the repository to return 0 credits for a user
- Call `TryDeductCreditAsync()` and verify it returns `false`
- **Note:** You may need to adjust the mock setup depending on your caching implementation

**Test 2: `ConcurrentDeductions_WithOneCredit_OnlyOneSucceeds`**
- This is the critical concurrency test
- Mock the repository to return 1 credit initially
- Create 10 concurrent tasks that all call `TryDeductCreditAsync()` for the same user
- Verify exactly 1 task returns `true` and 9 return `false`
- **Important:** This test validates your concurrency control implementation

## Test Users

The system includes these pre-configured test users:
- `user1`: 10 credits
- `user2`: 5 credits
- `user3`: 0 credits (should always return 402)
- `user4`: 100 credits

## Success Criteria

Your implementation is complete when:

1. All tests pass (`dotnet test` shows 8/8 passing)
2. `CreditService.TryDeductCreditAsync()` is fully implemented
3. Middleware is registered and working
4.  API blocks users with 0 credits (returns 402)
5.  Concurrency test passes: 10 requests with 1 credit = 1 success, 9 failures
6.  Response time is < 20ms (caching prevents 100ms database calls)

## Testing Your Implementation

### Run Tests
```bash
dotnet test
```

### Test the API
1. Start the API: `cd RivaAssessment && dotnet run`
2. Open Swagger: http://localhost:5000/swagger
3. Test endpoints with different `X-User-Id` headers:
   - `user1` (has credits) - should succeed
   - `user3` (no credits) - should return 402

### Manual Concurrency Test
```bash
# Make 10 concurrent requests for user1
for i in {1..10}; do
  curl -H "X-User-Id: user1" http://localhost:5000/api/weatherforecast &
done
wait
```

## Files to Modify

1. **`RivaAssessment/Services/CreditService.cs`**
   - Implement `TryDeductCreditAsync()` method

2. **`RivaAssessment/Middleware/CreditEnforcementMiddleware.cs`**
   - Complete the middleware logic (mostly done, just needs to work with your CreditService)

3. **`RivaAssessment/Program.cs`**
   - Uncomment: `builder.Services.AddSingleton<ICreditService, CreditService>();`
   - Uncomment: `app.UseMiddleware<CreditEnforcementMiddleware>();`

4. **`RivaAssessment.Tests/CreditServiceTests.cs`**
   - Complete the test implementations
   - Fix mock repository setups if needed
   - Ensure concurrency test properly creates 10 parallel tasks
   - Verify all assertions are correct

## Tips

- **Start with CreditService** - This is the core logic. Get it working before the middleware.
- **Use in-memory caching** - `ConcurrentDictionary` is perfect for this
- **Per-user locking** - Use `SemaphoreSlim` to prevent race conditions per user
- **Cache-aside pattern** - Check cache first, load from repository if cache miss
- **Test incrementally** - Run tests after each change to verify your progress

## Time Estimate

- **CreditService implementation:** 30-45 minutes
- **Test implementation:** 10-15 minutes
- **Middleware completion:** 10-15 minutes  
- **Testing & verification:** 10-15 minutes

**Total: 60-90 minutes**

Good luck! 🚀
