using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using testesIntegracao.context;


namespace testesIntegracao.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CursosController : Controller
    {
        //private readonly EstudoR2Context _context;

        public CursosController()
        {
            //_context = context;
        }

        [HttpGet("Buscar")] // GET: TCursoes
        public async Task<IActionResult> Buscar()
        {
            var conexao = new testeApiContext();
            var cursos = await conexao.Cursos.ToListAsync();
            return Ok(cursos);
        }

        [HttpGet("BuscarPorId/{id}")] // GET: TCursoes/Details/5
        public async Task<IActionResult> BuscarPorId(int? id)
        {
            var conexao = new testeApiContext();
            if (id == null)
            {
                return NotFound();
            }

            var tCurso = await conexao.Cursos.FirstOrDefaultAsync(m => m.IdCurso == id);
            if (tCurso == null)
            {
                return NotFound();
            }

            return Ok(tCurso);
        }


        [HttpPost("Criar")] // POST: TCursoes/Create
        public async Task<IActionResult> Criar(Curso tCurso)
        {
            try
            {
                var conexao = new testeApiContext();
                var usuarioExistente = await conexao.Cursos.FirstOrDefaultAsync(c => c.DescricaoCurso == tCurso.DescricaoCurso);

                if (usuarioExistente != null)
                {
                    return BadRequest("Curso já existe");
                }

                if (ModelState.IsValid)
                {
                    conexao.Cursos.Add(tCurso);
                    await conexao.SaveChangesAsync();
                }

                return Ok(tCurso);
            }
            catch (Exception e)
            {
                return BadRequest(tCurso);
            }
        }

        [HttpPut("Editar/{id}")] // PUT: TCursoes/Edit/5
        public async Task<IActionResult> Editar(int id, Curso tCurso)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var conexao = new testeApiContext();
                    conexao.Update(tCurso);
                    await conexao.SaveChangesAsync();
                    return Ok(tCurso);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TCursoExists(tCurso.IdCurso))
                    {
                        return NotFound();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }

            }

            return Ok(tCurso);
        }

        [HttpDelete("Delete/{id}")] // POST: TCursoes/Delete/5
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var conexao = new testeApiContext();
                var tCurso = await conexao.Cursos.FindAsync(id);

                if(tCurso == null) return NotFound();

                conexao.Cursos.Remove(tCurso);
                await conexao.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        private bool TCursoExists(int id)
            {
                var conexao = new testeApiContext();
                return conexao.Cursos.Any(e => e.IdCurso == id);
            }
        }
    }
