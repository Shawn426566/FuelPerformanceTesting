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
    $allEvaluationsList = @()
    $page = 1
    $pageSize = 200
    do {
        $response = Invoke-ApiCall -Method "GET" -Endpoint "/api/evaluations?page=$page&pageSize=$pageSize"
        if ($response.items) {
            $allEvaluationsList += $response.items
            $page++
        } else {
            break
        }
    } while ($true)
    foreach ($evaluation in $allEvaluationsList) {
        Write-Host "  Deleting evaluation (ID: $($evaluation.evaluationId))" -ForegroundColor Red
        Invoke-ApiCall -Method "DELETE" -Endpoint "/api/evaluations/$($evaluation.evaluationId)"
    }

    Write-Host "Fetching and deleting all players..." -ForegroundColor Yellow
    $allPlayersList = @()
    $page = 1
    do {
        $response = Invoke-ApiCall -Method "GET" -Endpoint "/api/players?page=$page&pageSize=$pageSize"
        if ($response.items) {
            $allPlayersList += $response.items
            $page++
        } else {
            break
        }
    } while ($true)
    foreach ($player in $allPlayersList) {
        Write-Host "  Deleting player: $($player.firstName) $($player.lastName) (ID: $($player.playerId))" -ForegroundColor Red
        Invoke-ApiCall -Method "DELETE" -Endpoint "/api/players/$($player.playerId)"
    }

    Write-Host "Fetching and deleting all staff members..." -ForegroundColor Yellow
    $allStaffList = @()
    $page = 1
    do {
        $response = Invoke-ApiCall -Method "GET" -Endpoint "/api/staffMembers?page=$page&pageSize=$pageSize"
        if ($response.items) {
            $allStaffList += $response.items
            $page++
        } else {
            break
        }
    } while ($true)
    foreach ($staff in $allStaffList) {
        Write-Host "  Deleting staff: $($staff.firstName) $($staff.lastName) (ID: $($staff.staffMemberID))" -ForegroundColor Red
        Invoke-ApiCall -Method "DELETE" -Endpoint "/api/staffMembers/$($staff.staffMemberID)"
    }

    Write-Host "Fetching and deleting all teams..." -ForegroundColor Yellow
    $allTeamsList = @()
    $page = 1
    do {
        $response = Invoke-ApiCall -Method "GET" -Endpoint "/api/teams?page=$page&pageSize=$pageSize"
        if ($response.items) {
            $allTeamsList += $response.items
            $page++
        } else {
            break
        }
    } while ($true)
    foreach ($team in $allTeamsList) {
        if ($team.name -match "(Phoenix Rising|Mountain Lions|Thunder Hawks|Northern Wolves|Silver Eagles)") {
            Write-Host "  Deleting team: $($team.name) (ID: $($team.teamID))" -ForegroundColor Red
            Invoke-ApiCall -Method "DELETE" -Endpoint "/api/teams/$($team.teamID)"
        }
    }

    Write-Host "Fetching and deleting associations..." -ForegroundColor Yellow
    $allAssocsList = @()
    $page = 1
    do {
        $response = Invoke-ApiCall -Method "GET" -Endpoint "/api/associations?page=$page&pageSize=$pageSize"
        if ($response.items) {
            $allAssocsList += $response.items
            $page++
        } else {
            break
        }
    } while ($true)
    foreach ($assoc in $allAssocsList) {
        if ($Associations -contains $assoc.name) {
            Write-Host "Deleting association: $($assoc.name) (ID: $($assoc.associationID))" -ForegroundColor Red
            Invoke-ApiCall -Method "DELETE" -Endpoint "/api/associations/$($assoc.associationID)"
        }
    }
    
    Write-Host "Database cleanup completed successfully!" -ForegroundColor Green
}
catch {
    Write-Host "Cleanup failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
