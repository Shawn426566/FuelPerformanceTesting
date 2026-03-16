# Database Seeding Script for Fuel Performance Testing
# Creates associations, age-group teams, players, staff members, and evaluations

$ApiBaseUrl = "http://localhost:5114"
$ErrorActionPreference = "Stop"

# Configuration
$Associations = @("Phoenix Rising", "Mountain Lions", "Thunder Hawks", "Northern Wolves", "Silver Eagles")
$AgeGroups = @(
    @{ name = "u6"; birthYear = 2019; birthMonth = 9 },
    @{ name = "u8"; birthYear = 2017; birthMonth = 9 },
    @{ name = "u10"; birthYear = 2015; birthMonth = 9 },
    @{ name = "u12"; birthYear = 2013; birthMonth = 9 },
    @{ name = "u14"; birthYear = 2011; birthMonth = 9 },
    @{ name = "u16"; birthYear = 2009; birthMonth = 9 },
    @{ name = "u18"; birthYear = 2007; birthMonth = 9 }
)

$PositionConfig = @(
    @{ position = "RW"; count = 3 },
    @{ position = "C"; count = 3 },
    @{ position = "LW"; count = 3 },
    @{ position = "D"; count = 6 },
    @{ position = "G"; count = 2 }
)

# Helper functions
$script:GlobalPlayerIndex = 0

function Get-FirstName {
    param([int]$index)
    $firstNames = @(
        "Alex", "Jordan", "Casey", "Morgan", "Riley", "Taylor", "Cameron", "Dakota",
        "Kyle", "Blake", "Adrian", "Owen", "Evan", "Mason", "Liam", "Noah",
        "Jackson", "Aiden", "Ethan", "Logan", "Benjamin", "Lucas", "Henry", "Alexander",
        "Michael", "Jacob", "James", "Daniel", "Matthew", "David", "Joseph", "Samuel",
        "Christopher", "Anthony", "Ryan", "Brandon", "Nicholas", "Robert", "Justin", "Kevin",
        "Brian", "Paul", "Stephen", "Larry", "Jason", "Jeffrey", "Mark", "Gary",
        "Ashley", "Jessica", "Emily", "Madison", "Sarah", "Samantha", "Rebecca", "Sophia",
        "Olivia", "Ava", "Isabella", "Hannah", "Amelia", "Evelyn", "Harper", "Charlotte",
        "Emma", "Mia", "Abigail", "Grace", "Chloe", "Lily", "Zoe", "Sofia"
    )
    return $firstNames[$index % $firstNames.Count]
}

function Get-LastName {
    param([int]$index)
    $lastNames = @(
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
        "Rodriguez", "Martinez", "Anderson", "Taylor", "Thomas", "Moore", "Jackson", "Martin",
        "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez",
        "Lewis", "Robinson", "Young", "Allen", "King", "Wright", "Scott", "Torres",
        "Peterson", "Phillips", "Campbell", "Parker", "Evans", "Edwards", "Collins", "Reyes",
        "Stewart", "Morris", "Morales", "Murphy", "Cook", "Rogers", "Morgan", "Peterson",
        "Cooper", "Reed", "Bell", "Gomez", "Murray", "Freeman", "Wells", "Webb",
        "Simpson", "Stevens", "Tucker", "Porter", "Hunter", "Hicks", "Crawford", "Henry"
    )
    return $lastNames[$index % $lastNames.Count]
}

function Get-UniqueFirstName {
    $index = $script:GlobalPlayerIndex
    return Get-FirstName -index $index
}

function Get-UniqueLastName {
    $index = $script:GlobalPlayerIndex
    $script:GlobalPlayerIndex++
    return Get-LastName -index $index
}

function New-StaffMember {
    param(
        [string]$FirstName,
        [string]$LastName,
        [string]$Role,
        [int]$TeamId,
        [int]$AssociationId
    )
    
    $email = "$($FirstName.ToLower()).$($LastName.ToLower().Replace(' ', ''))@example.com"
    
    $body = @{
        firstName = $FirstName
        lastName = $LastName
        email = $email
        role = $Role
    }
    
    if ($TeamId -gt 0) {
        $body.teamID = $TeamId
    }
    if ($AssociationId -gt 0) {
        $body.associationID = $AssociationId
    }
    
    return $body | ConvertTo-Json
}

function New-Evaluation {
    param(
        [int]$PlayerId,
        [int]$StaffMemberId,
        [DateTime]$EvaluationDate,
        [string]$Summary
    )
    
    $body = @{
        playerId = $PlayerId
        staffMemberId = $StaffMemberId
        date = $EvaluationDate.ToString("yyyy-MM-dd")
        summary = $Summary
    } | ConvertTo-Json
    
    return $body
}


function New-Player {
    param(
        [string]$FirstName,
        [string]$LastName,
        [string]$Position,
        [int]$BirthYear,
        [int]$BirthMonth,
        [int]$TeamId,
        [int]$AssociationId
    )
    
    $email = "$($FirstName.ToLower()).$($LastName.ToLower())@example.com"
    $birthDate = [DateTime]::new($BirthYear, $BirthMonth, 15)
    
    $body = @{
        firstName = $FirstName
        lastName = $LastName
        position = $Position
        birthDate = $birthDate.ToString("yyyy-MM-dd")
        email = $email
        teamID = $TeamId
        associationID = $AssociationId
        jerseyNumber = Get-Random -Minimum 1 -Maximum 100
    } | ConvertTo-Json
    
    return $body
}

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

# Main execution
Write-Host "Starting database seeding..." -ForegroundColor Cyan

try {
    Write-Host "Creating associations..." -ForegroundColor Yellow
    $createdAssociations = @()
    
    foreach ($assocName in $Associations) {
        $body = @{ name = $assocName } | ConvertTo-Json
        $assoc = Invoke-ApiCall -Method "POST" -Endpoint "/api/associations" -Body $body
        $createdAssociations += $assoc
        Write-Host "  Created association: $assocName (ID: $($assoc.associationID))" -ForegroundColor Green
    }
    
    foreach ($assoc in $createdAssociations) {
        Write-Host "Processing association: $($assoc.name)" -ForegroundColor Cyan
        
        # Create association manager
        $firstName = Get-UniqueFirstName
        $lastName = Get-UniqueLastName
        $staffBody = New-StaffMember -FirstName $firstName -LastName $lastName -Role "Manager" -TeamId 0 -AssociationId $assoc.associationID
        $staff = Invoke-ApiCall -Method "POST" -Endpoint "/api/staffMembers" -Body $staffBody
        Write-Host "  Created association manager: $($staff.firstName) $($staff.lastName)" -ForegroundColor Cyan
        
        foreach ($ageGroup in $AgeGroups) {
            $teamBody = @{
                name = "$($assoc.name) $($ageGroup.name)"
                associationID = $assoc.associationID
            } | ConvertTo-Json
            
            $team = Invoke-ApiCall -Method "POST" -Endpoint "/api/teams" -Body $teamBody
            Write-Host "  Created team: $($team.name) (ID: $($team.teamID))" -ForegroundColor Green
            
            # Create team staff (Coach, Assistant, Manager)
            $coachFirst = Get-UniqueFirstName
            $coachLast = Get-UniqueLastName
            $coachBody = New-StaffMember -FirstName $coachFirst -LastName $coachLast -Role "Coach" -TeamId $team.teamID -AssociationId 0
            $coach = Invoke-ApiCall -Method "POST" -Endpoint "/api/staffMembers" -Body $coachBody
            Write-Host "    Created coach: $($coach.firstName) $($coach.lastName)" -ForegroundColor Cyan
            
            $assistantFirst = Get-UniqueFirstName
            $assistantLast = Get-UniqueLastName
            $assistantBody = New-StaffMember -FirstName $assistantFirst -LastName $assistantLast -Role "Assistant" -TeamId $team.teamID -AssociationId 0
            $assistant = Invoke-ApiCall -Method "POST" -Endpoint "/api/staffMembers" -Body $assistantBody
            Write-Host "    Created assistant: $($assistant.firstName) $($assistant.lastName)" -ForegroundColor Cyan
            
            $managerFirst = Get-UniqueFirstName
            $managerLast = Get-UniqueLastName
            $managerBody = New-StaffMember -FirstName $managerFirst -LastName $managerLast -Role "Manager" -TeamId $team.teamID -AssociationId 0
            $manager = Invoke-ApiCall -Method "POST" -Endpoint "/api/staffMembers" -Body $managerBody
            Write-Host "    Created team manager: $($manager.firstName) $($manager.lastName)" -ForegroundColor Cyan
            
            # Create players for this team
            foreach ($posConfig in $PositionConfig) {
                for ($i = 0; $i -lt $posConfig.count; $i++) {
                    $firstName = Get-UniqueFirstName
                    $lastName = Get-UniqueLastName
                    
                    $playerBody = New-Player -FirstName $firstName -LastName $lastName -Position $posConfig.position -BirthYear $ageGroup.birthYear -BirthMonth $ageGroup.birthMonth -TeamId $team.teamID -AssociationId $assoc.associationID
                    
                    $player = Invoke-ApiCall -Method "POST" -Endpoint "/api/players" -Body $playerBody
                    Write-Host "    Created player: $firstName $lastName ($($posConfig.position))" -ForegroundColor Green
                }
            }
        }
    }
    
    # Create independent evaluators
    Write-Host "Creating independent evaluators..." -ForegroundColor Yellow
    $evaluators = @()
    for ($i = 1; $i -le 20; $i++) {
        $firstName = Get-UniqueFirstName
        $lastName = Get-UniqueLastName
        
        $evalBody = New-StaffMember -FirstName $firstName -LastName $lastName -Role "Evaluator" -TeamId 0 -AssociationId 0
        $evaluator = Invoke-ApiCall -Method "POST" -Endpoint "/api/staffMembers" -Body $evalBody
        $evaluators += $evaluator
        Write-Host "  Created evaluator: $($evaluator.firstName) $($evaluator.lastName)" -ForegroundColor Cyan
    }
    
    # Create evaluations for players
    Write-Host "Creating evaluations..." -ForegroundColor Yellow
    $allPlayersList = @()
    $page = 1
    $pageSize = 200
    do {
        $response = Invoke-ApiCall -Method "GET" -Endpoint "/api/players?page=$page&pageSize=$pageSize"
        if ($response.items) {
            $allPlayersList += $response.items
            $page++
        } else {
            break
        }
    } while ($true)

    $evalCount = 0
    foreach ($player in $allPlayersList) {
        if ($player.playerId -ne 0) {
            for ($eval = 0; $eval -lt 3; $eval++) {
                $randomEvaluator = $evaluators | Get-Random
                $randomDays = Get-Random -Minimum -90 -Maximum 0
                $evalDate = (Get-Date).AddDays($randomDays)
                $summaries = @(
                    "Strong performance across all areas",
                    "Needs improvement on positioning",
                    "Excellent effort and technique",
                    "Good fundamentals, keep working",
                    "Outstanding game sense",
                    "Focus on skill development",
                    "Great defensive play",
                    "Continue building conditioning",
                    "Solid contribution to team",
                    "Working well with teammates"
                )
                $randomSummary = $summaries | Get-Random
                $evalBody = New-Evaluation -PlayerId $player.playerId -StaffMemberId $randomEvaluator.staffMemberID -EvaluationDate $evalDate -Summary $randomSummary
                $evaluation = Invoke-ApiCall -Method "POST" -Endpoint "/api/evaluations" -Body $evalBody
                $evalCount++
            }
        } else {
            Write-Host "Skipping evaluation for player with invalid playerID (0)" -ForegroundColor Red
        }
    }
    Write-Host "Created 3 evaluations per player ($evalCount total)" -ForegroundColor Cyan
    
    Write-Host "Database seeding completed successfully!" -ForegroundColor Green
}
catch {
    Write-Host "Seeding failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
