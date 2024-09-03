const { execSync } = require('child_process');

function analyzeTestCommits(repoPath, countAllCommits = false) {
    const results = {};

    // Get total commit count
    if (countAllCommits) {
        results.totalCommits = parseInt(execSync(`git --git-dir=${repoPath}/.git rev-list --count HEAD`).toString());
    }

    // Get test-related commits and analyze
    const logOutput = execSync(`git --git-dir=${repoPath}/.git log --name-only`, { maxBuffer: 1024 * 1024 * 10 }).toString(); // Increase the buffer size: ;
    const commits = logOutput.split('\n\n');

    const testCommits = commits.filter(commit => {
        const files = commit.split('\n');
        return files.some(file => file.includes('test') || file.includes('spec') || file.includes('testcase'));
    });

    // Count test commits
    results.testCommits = testCommits.length;

    // Get top contributors for test-related commits
    const topContributors = execSync(`git --git-dir=/Users/igorwestermann/Dev/UFJF/Topicos/antlr4/.git shortlog -sn --grep="test"`).toString();
    results.topContributors = topContributors.split('\n').map(line => {
        const [count, author] = line.split('\t');
        return { author, count: count };
    });
    // Add more metrics as needed (e.g., first/last commit date, frequently modified files)

    return results;
}

// Example usage
const repoPath = '/Users/igorwestermann/Dev/UFJF/Topicos/antlr4';
const analysis = analyzeTestCommits(repoPath, true); // Count all commits

console.log('Total commits:', analysis.totalCommits);
console.log('Test-related commits:', analysis.testCommits);
console.log('Top contributors:', analysis.topContributors);