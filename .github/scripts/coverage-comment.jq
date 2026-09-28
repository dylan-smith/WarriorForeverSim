# Renders ReportGenerator's JsonSummary (Summary.json) as the compact markdown that
# CI posts as the PR coverage comment and job summary:
#
#   ## 📊 Code Coverage
#   | Area | Lines | Branches |   one row per assembly
#   <details> ... </details>      per-assembly table of every class, collapsed
#
# Usage: jq -r -f .github/scripts/coverage-comment.jq coverage-report/Summary.json

# Percentages come from ReportGenerator so they match the HTML report exactly.
# A class with no branches has a null branch percentage.
def pct(percent; covered; total):
  if percent == null then "n/a" else "\(percent)% (\(covered)/\(total))" end;

def row(name):
  "| \(name) | \(pct(.coverage; .coveredlines; .coverablelines)) | \(pct(.branchcoverage; .coveredbranches; .totalbranches)) |";

"## 📊 Code Coverage",
"",
"| Area | Lines | Branches |",
"|:---|---:|---:|",
(.coverage.assemblies[] | row(.name)),
"",
(.coverage.assemblies[]
  | .name as $assembly
  | "<details><summary>\($assembly) coverage by class</summary>",
    "",
    "| Class | Lines | Branches |",
    "|:---|---:|---:|",
    (.classesinassembly[] | row(.name | ltrimstr($assembly + "."))),
    "",
    "</details>")
