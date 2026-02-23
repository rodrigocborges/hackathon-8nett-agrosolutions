Write-Host "🚀 Iniciando os Port-Forwards do AgroSolutions..." -ForegroundColor Green

# Array para guardar os processos e podermos encerrá-los depois
$processos = @()

# Inicia cada port-forward minimizado para não poluir a tela e guarda a referência (-PassThru)
$processos += Start-Process -FilePath "kubectl" -ArgumentList "port-forward service/rabbitmq 15672:15672" -PassThru -WindowStyle Minimized
$processos += Start-Process -FilePath "kubectl" -ArgumentList "port-forward service/agro-identity-service 8081:80" -PassThru -WindowStyle Minimized
$processos += Start-Process -FilePath "kubectl" -ArgumentList "port-forward service/agro-management-service 8082:80" -PassThru -WindowStyle Minimized
$processos += Start-Process -FilePath "kubectl" -ArgumentList "port-forward service/agro-ingestion-service 8083:80" -PassThru -WindowStyle Minimized
$processos += Start-Process -FilePath "kubectl" -ArgumentList "port-forward service/agro-alerts-service 8084:80" -PassThru -WindowStyle Minimized

Write-Host "✅ Todos os serviços estão mapeados e rodando em background!" -ForegroundColor Cyan
Write-Host "--------------------------------------------------------"
Write-Host " 🐇 RabbitMQ   -> http://localhost:15672 (guest/guest)"
Write-Host " 🔐 Identity   -> http://localhost:8081/swagger"
Write-Host " 🌾 Management -> http://localhost:8082/swagger"
Write-Host " 📡 Ingestion  -> http://localhost:8083/swagger"
Write-Host " 🚨 Alerts     -> http://localhost:8084/swagger"
Write-Host "--------------------------------------------------------"
Write-Host "Pressione [ENTER] nesta janela para encerrar todas as conexões..." -ForegroundColor Yellow

# Pausa a execução aguardando o usuário
Read-Host

Write-Host "Encerrando conexões..." -ForegroundColor Red

# Mata todos os processos de port-forward que abrimos
foreach ($p in $processos) {
    Stop-Process -Id $p.Id -Force -ErrorAction SilentlyContinue
}

Write-Host "Conexões encerradas com sucesso! Bom código." -ForegroundColor Green