# Contributing to Universal Control Remapper

Thank you for taking your time to contribute to Universal Control Remapper!

The following is a set of guidelines for contributing to Universal Control Remapper. These are mostly guidelines, not rules. Please use your best judgement when contributing and feel free to contact us if you have any questions.



## Contributing

1. Fork Universal Control Remapper on Github
2. Create a new branch for your change, refer to [Branching](#branching)
3. Commit changes to your own repository following the [Commit guidelines](#commit-guidelines)
4. Note your changes in the `CHANGELOG.md`
5. Create a pull request for your new branch targeting `UCR/develop`, refer to [Pull requests](#pull-requests) 




## Branching

This repository is using GitFlow as branching strategy which means features and hotfixes are handled with branches. Contributions directly on the `develop` branch is only for minor changes and contributions directly to the `master` branch is prohibited, as it is considered the release branch. The name of your branch should be prefixed with one of the following prefixes depending on your change:

- `feature/`: Prefix when you are **adding** new functionality
- `hotfix/`: Prefix when you are **fixing** existing functionality



The name of your branch, following the prefix, should clearly indicate what is changing



## Commit guidelines

Commit message are just as important as the code it describes as it describes what and why the codebase has changed. Your commit messages should adhere to the following guidelines:

- Use the present tense ("Add feature" not "Added feature")
- Use the imperative mood ("Move cursor to..." not "Moves cursor to...")
- Limit the first line to 72 characters or less
- Reference issues and pull requests liberally after the first line




## Pull requests

Create a new pull request targeting `UCR/develop` when your `branch` is ready to be added. The pull request title should describe your change and the description should describe what has changed and why it was changed. Reference any issues or related pull requests in the description, if any.

Pull request are checked by quality gates which needs to be passed before it is considered for merge. The change must build on the continuous integration Appveyor and any issues found by SonarQube should be fixed. Any required change should be committed on your own branch until quality gates are passing.

Your pull request are then code reviewed and eventually merged. Your changes are then released as part of the next release.