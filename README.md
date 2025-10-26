
## Considerations 
We will be considering the following:
- What does your code look like?
-- Can you write clean (readable) code?
- What sort of testing do you do?
--- Do you know how to write tests?
-- Do you do Test Driven Development (TDD)?
- Is your solution pragmatic?
-- Can you write a simple, readable, and pragmatic solution, rather than a fancy and difficult-to-read and maintain solution?
- How are your software engineering skills?
-- Does your submission actually solve the bulk of the problem?


## Code Quality
- Keep your code clean, concise, and domain-focused — use structures that represent real concepts (e.g. Game, Set, Match).
- Split long functions into smaller, self-documenting ones.
- Avoid duplicate files or logic (e.g. don’t maintain both states.ts and types.ts if they serve the same purpose).
- Use early returns instead of deep if/else chains to simplify flow.
- Keep folder structure logical — e.g. use a src directory instead of lumping everything under helpers/.

## Testing
- Ensure test coverage matches code complexity — more code should mean more tests, not the other way around.
- Write unit tests for core functionality; don’t rely only on integration tests.
- Tests should explain the code, not obscure it — avoid overly opaque test data or setup.
- Keep tests focused and readable: only assert what’s necessary.
- Don’t overtest trivial helpers or tiny functions.
- Group related tests logically; avoid unnecessary separate files for edge cases.

## Documentation
- A clear README that explains structure and purpose goes a long way.
- Use tests as documentation — make them descriptive and easy to follow.

## Overall
- Strive for clarity and proportionality — clean design, right amount of code, right amount of tests.
- Avoid signs of “AI slop” or overgeneration — every line should serve a clear purpose.