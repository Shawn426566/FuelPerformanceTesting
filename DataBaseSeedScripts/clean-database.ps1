# Database Cleanup Script
# Deletes all players, teams, and associations

$ApiBaseUrl = "http://localhost:5114"
$ErrorActionPreference = "Stop"

$Associations = @("Phoenix Rising", "Mountain Lions", "Thunder Hawks", "Northern Wolves", "Silver Eagles")

function Invoke-ApiCall {
    param(
        [string]$Method,
        [string]$Endpoint,
        [string]$Body
    )
    
    $url = "$ApiBaseUrl$Endpoint"
    $params = @{
        Uri = $url
        Method = $Method
        ContentType = "application/json"
        ErrorAction = "Stop"
    }
    
    if ($Body) {
        $params.Body = $Body
    }
    
    try {
        $response = Invoke-RestMethod @params
        return $response
    }
    catch {
        Write-Host "Error calling $Method $url : $($_.Exception.Message)" -ForegroundColor Red
        throw
    }
}

Write-Host "Starting database cleanup..." -ForegroundColor Cyan

try {
    Write-Host "Fetching and deleting all evaluations..." -ForegroundColor Yellow
    $allEvaluations = Invoke-ApiCall -Method "GET" -Endpoint "/api/evaluations"
    foreach ($evaluation in $allEvaluations) {
        Write-Host "  Deleting evaluation (ID: $($evaluation.evaluationId))" -ForegroundColor Red
        Invoke-ApiCall -Method "DELETE" -Endpoint "/api/evaluations/$($evaluation.evaluationId)"
    }
    
    Write-Host "Fetching and deleting all players..." -ForegroundColor Yellow
    $allPlayers = Invoke-ApiCall -Method "GET" -Endpoint "/api/players"
    foreach ($player in $allPlayers) {
        Write-Host "  Deleting player: $($player.firstName) $($player.lastName) (ID: $($player.playerID))" -ForegroundColor Red
        Invoke-ApiCall -Method "DELETE" -Endpoint "/api/players/$($player.playerID)"
    }
    
    Write-Host "Fetching and deleting all staff members..." -ForegroundColor Yellow
    $allStaff = Invoke-ApiCall -Method "GET" -Endpoint "/api/staffMembers"
    foreach ($staff in $allStaff) {
        Write-Host "  Deleting staff: $($staff.firstName) $($staff.lastName) (ID: $($staff.staffMemberID))" -ForegroundColor Red
        Invoke-ApiCall -Method "DELETE" -Endpoint "/api/staffMembers/$($staff.staffMemberID)"
    }
    
    Write-Host "Fetching and deleting all teams..." -ForegroundColor Yellow
    $allTeams = Invoke-ApiCall -Method "GET" -Endpoint "/api/teams"
    foreach ($team in $allTeams) {
        if ($team.name -match "(Phoenix Rising|Mountain Lions|Thunder Hawks|Northern Wolves|Silver Eagles)") {
            Write-Host "  Deleting team: $($team.name) (ID: $($team.teamID))" -ForegroundColor Red
            Invoke-ApiCall -Method "DELETE" -Endpoint "/api/teams/$($team.teamID)"
        }
    }
    
    Write-Host "Fetching and deleting associations..." -ForegroundColor Yellow
    $allAssocs = Invoke-ApiCall -Method "GET" -Endpoint "/api/association"
    
    foreach ($assoc in $allAssocs) {
        if ($Associations -contains $assoc.name) {
            Write-Host "Deleting association: $($assoc.name) (ID: $($assoc.associationID))" -ForegroundColor Red
            Invoke-ApiCall -Method "DELETE" -Endpoint "/api/association/$($assoc.associationID)"
        }
    }
    
    Write-Host "Database cleanup completed successfully!" -ForegroundColor Green
}
catch {
    Write-Host "Cleanup failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
