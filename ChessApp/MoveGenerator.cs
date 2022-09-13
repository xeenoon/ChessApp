using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp
{
    public class MoveGenerator
    {
        private const ulong NO_TOP_ROW    = 72057594037927935UL;
        private const ulong NO_BOTTOM_ROW = 18446744073709551360UL;
        private const ulong FULL          = 18446744073709551615UL;
        private const ulong NO_RIGHT_COLLUMN = 9187201950435737471UL;
        private const ulong NO_LEFT_COLLUMN  = 18374403900871474942;

        private static readonly ulong[] blackPawnAttack = new ulong[64] { 144115188075855872UL, 360287970189639680UL, 720575940379279360UL, 1441151880758558720UL, 2882303761517117440UL, 5764607523034234880UL, 11529215046068469760UL, 4611686018427387904UL, 2UL, 5UL, 10UL, 20UL, 40UL, 80UL, 160UL, 64UL, 512UL, 1280UL, 2560UL, 5120UL, 10240UL, 20480UL, 40960UL, 16384UL, 131072UL, 327680UL, 655360UL, 1310720UL, 2621440UL, 5242880UL, 10485760UL, 4194304UL, 33554432UL, 83886080UL, 167772160UL, 335544320UL, 671088640UL, 1342177280UL, 2684354560UL, 1073741824UL, 8589934592UL, 21474836480UL, 42949672960UL, 85899345920UL, 171798691840UL, 343597383680UL, 687194767360UL, 274877906944UL, 2199023255552UL, 5497558138880UL, 10995116277760UL, 21990232555520UL, 43980465111040UL, 87960930222080UL, 175921860444160UL, 70368744177664UL, 562949953421312UL, 1407374883553280UL, 2814749767106560UL, 5629499534213120UL, 11258999068426240UL, 22517998136852480UL, 45035996273704960UL, 18014398509481984UL, };

        private static readonly ulong[] whitePawnAttack = new ulong[64] { 512UL, 1280UL, 2560UL, 5120UL, 10240UL, 20480UL, 40960UL, 16384UL, 131072UL, 327680UL, 655360UL, 1310720UL, 2621440UL, 5242880UL, 10485760UL, 4194304UL, 33554432UL, 83886080UL, 167772160UL, 335544320UL, 671088640UL, 1342177280UL, 2684354560UL, 1073741824UL, 8589934592UL, 21474836480UL, 42949672960UL, 85899345920UL, 171798691840UL, 343597383680UL, 687194767360UL, 274877906944UL, 2199023255552UL, 5497558138880UL, 10995116277760UL, 21990232555520UL, 43980465111040UL, 87960930222080UL, 175921860444160UL, 70368744177664UL, 562949953421312UL, 1407374883553280UL, 2814749767106560UL, 5629499534213120UL, 11258999068426240UL, 22517998136852480UL, 45035996273704960UL, 18014398509481984UL, 144115188075855872UL, 360287970189639680UL, 720575940379279360UL, 1441151880758558720UL, 2882303761517117440UL, 5764607523034234880UL, 11529215046068469760UL, 4611686018427387904UL, 2UL, 5UL, 10UL, 20UL, 40UL, 80UL, 160UL, 64UL, };
        private static readonly ulong[] blackPawnNoAttack = new ulong[64] { 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 1UL, 2UL, 4UL, 8UL, 16UL, 32UL, 64UL, 128UL, 256UL, 512UL, 1024UL, 2048UL, 4096UL, 8192UL, 16384UL, 32768UL, 65536UL, 131072UL, 262144UL, 524288UL, 1048576UL, 2097152UL, 4194304UL, 8388608UL, 16777216UL, 33554432UL, 67108864UL, 134217728UL, 268435456UL, 536870912UL, 1073741824UL, 2147483648UL, 4294967296UL, 8589934592UL, 17179869184UL, 34359738368UL, 68719476736UL, 137438953472UL, 274877906944UL, 549755813888UL, 1103806595072UL, 2207613190144UL, 4415226380288UL, 8830452760576UL, 17660905521152UL, 35321811042304UL, 70643622084608UL, 141287244169216UL, 281474976710656UL, 562949953421312UL, 1125899906842624UL, 2251799813685248UL, 4503599627370496UL, 9007199254740992UL, 18014398509481984UL, 36028797018963968UL };
        private static readonly ulong[] whitePawnNoAttack = new ulong[64] { 256UL, 512UL, 1024UL, 2048UL, 4096UL, 8192UL, 16384UL, 32768UL, 16842752UL, 33685504UL, 67371008UL, 134742016UL, 269484032UL, 538968064UL, 1077936128UL, 2155872256UL, 16777216UL, 33554432UL, 67108864UL, 134217728UL, 268435456UL, 536870912UL, 1073741824UL, 2147483648UL, 4294967296UL, 8589934592UL, 17179869184UL, 34359738368UL, 68719476736UL, 137438953472UL, 274877906944UL, 549755813888UL, 1099511627776UL, 2199023255552UL, 4398046511104UL, 8796093022208UL, 17592186044416UL, 35184372088832UL, 70368744177664UL, 140737488355328UL, 281474976710656UL, 562949953421312UL, 1125899906842624UL, 2251799813685248UL, 4503599627370496UL, 9007199254740992UL, 18014398509481984UL, 36028797018963968UL, 72057594037927936UL, 144115188075855872UL, 288230376151711744UL, 576460752303423488UL, 1152921504606846976UL, 2305843009213693952UL, 4611686018427387904UL, 9223372036854775808UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL };
        private static readonly ulong[] upRight = new ulong[64] { 9241421688590303744UL, 36099303471055872UL, 141012904183808UL, 550831656960UL, 2151686144UL, 8404992UL, 32768UL, 0UL, 4620710844295151616UL, 9241421688590303232UL, 36099303471054848UL, 141012904181760UL, 550831652864UL, 2151677952UL, 8388608UL, 0UL, 2310355422147510272UL, 4620710844295020544UL, 9241421688590041088UL, 36099303470530560UL, 141012903133184UL, 550829555712UL, 2147483648UL, 0UL, 1155177711056977920UL, 2310355422113955840UL, 4620710844227911680UL, 9241421688455823360UL, 36099303202095104UL, 141012366262272UL, 549755813888UL, 0UL, 577588851233521664UL, 1155177702467043328UL, 2310355404934086656UL, 4620710809868173312UL, 9241421619736346624UL, 36099165763141632UL, 140737488355328UL, 0UL, 288793326105133056UL, 577586652210266112UL, 1155173304420532224UL, 2310346608841064448UL, 4620693217682128896UL, 9241386435364257792UL, 36028797018963968UL, 0UL, 144115188075855872UL, 288230376151711744UL, 576460752303423488UL, 1152921504606846976UL, 2305843009213693952UL, 4611686018427387904UL, 9223372036854775808UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL };

        private static readonly ulong[] upLeft = new ulong[64] { 0UL, 256UL, 66048UL, 16909312UL, 4328785920UL, 1108169199616UL, 283691315109888UL, 72624976668147712UL, 0UL, 65536UL, 16908288UL, 4328783872UL, 1108169195520UL, 283691315101696UL, 72624976668131328UL, 145249953336262656UL, 0UL, 16777216UL, 4328521728UL, 1108168671232UL, 283691314053120UL, 72624976666034176UL, 145249953332068352UL, 290499906664136704UL, 0UL, 4294967296UL, 1108101562368UL, 283691179835392UL, 72624976397598720UL, 145249952795197440UL, 290499905590394880UL, 580999811180789760UL, 0UL, 1099511627776UL, 283673999966208UL, 72624942037860352UL, 145249884075720704UL, 290499768151441408UL, 580999536302882816UL, 1161999072605765632UL, 0UL, 281474976710656UL, 72620543991349248UL, 145241087982698496UL, 290482175965396992UL, 580964351930793984UL, 1161928703861587968UL, 2323857407723175936UL, 0UL, 72057594037927936UL, 144115188075855872UL, 288230376151711744UL, 576460752303423488UL, 1152921504606846976UL, 2305843009213693952UL, 4611686018427387904UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL };
        private static readonly ulong[] downLeft = new ulong[64] { 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 1UL, 2UL, 4UL, 8UL, 16UL, 32UL, 64UL, 0UL, 256UL, 513UL, 1026UL, 2052UL, 4104UL, 8208UL, 16416UL, 0UL, 65536UL, 131328UL, 262657UL, 525314UL, 1050628UL, 2101256UL, 4202512UL, 0UL, 16777216UL, 33619968UL, 67240192UL, 134480385UL, 268960770UL, 537921540UL, 1075843080UL, 0UL, 4294967296UL, 8606711808UL, 17213489152UL, 34426978560UL, 68853957121UL, 137707914242UL, 275415828484UL, 0UL, 1099511627776UL, 2203318222848UL, 4406653222912UL, 8813306511360UL, 17626613022976UL, 35253226045953UL, 70506452091906UL, 0UL, 281474976710656UL, 564049465049088UL, 1128103225065472UL, 2256206466908160UL, 4512412933881856UL, 9024825867763968UL, 18049651735527937UL };
        private static readonly ulong[] downRight = new ulong[64] { 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 2UL, 4UL, 8UL, 16UL, 32UL, 64UL, 128UL, 0UL, 516UL, 1032UL, 2064UL, 4128UL, 8256UL, 16512UL, 32768UL, 0UL, 132104UL, 264208UL, 528416UL, 1056832UL, 2113664UL, 4227072UL, 8388608UL, 0UL, 33818640UL, 67637280UL, 135274560UL, 270549120UL, 541097984UL, 1082130432UL, 2147483648UL, 0UL, 8657571872UL, 17315143744UL, 34630287488UL, 69260574720UL, 138521083904UL, 277025390592UL, 549755813888UL, 0UL, 2216338399296UL, 4432676798592UL, 8865353596928UL, 17730707128320UL, 35461397479424UL, 70918499991552UL, 140737488355328UL, 0UL, 567382630219904UL, 1134765260439552UL, 2269530520813568UL, 4539061024849920UL, 9078117754732544UL, 18155135997837312UL, 36028797018963968UL, 0UL };
        private static readonly ulong[] up = new ulong[64] { 72340172838076672UL, 144680345676153344UL, 289360691352306688UL, 578721382704613376UL, 1157442765409226752UL, 2314885530818453504UL, 4629771061636907008UL, 9259542123273814016UL, 72340172838076416UL, 144680345676152832UL, 289360691352305664UL, 578721382704611328UL, 1157442765409222656UL, 2314885530818445312UL, 4629771061636890624UL, 9259542123273781248UL, 72340172838010880UL, 144680345676021760UL, 289360691352043520UL, 578721382704087040UL, 1157442765408174080UL, 2314885530816348160UL, 4629771061632696320UL, 9259542123265392640UL, 72340172821233664UL, 144680345642467328UL, 289360691284934656UL, 578721382569869312UL, 1157442765139738624UL, 2314885530279477248UL, 4629771060558954496UL, 9259542121117908992UL, 72340168526266368UL, 144680337052532736UL, 289360674105065472UL, 578721348210130944UL, 1157442696420261888UL, 2314885392840523776UL, 4629770785681047552UL, 9259541571362095104UL, 72339069014638592UL, 144678138029277184UL, 289356276058554368UL, 578712552117108736UL, 1157425104234217472UL, 2314850208468434944UL, 4629700416936869888UL, 9259400833873739776UL, 72057594037927936UL, 144115188075855872UL, 288230376151711744UL, 576460752303423488UL, 1152921504606846976UL, 2305843009213693952UL, 4611686018427387904UL, 9223372036854775808UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL };
        private static readonly ulong[] down = new ulong[64] { 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 1UL, 2UL, 4UL, 8UL, 16UL, 32UL, 64UL, 128UL, 257UL, 514UL, 1028UL, 2056UL, 4112UL, 8224UL, 16448UL, 32896UL, 65793UL, 131586UL, 263172UL, 526344UL, 1052688UL, 2105376UL, 4210752UL, 8421504UL, 16843009UL, 33686018UL, 67372036UL, 134744072UL, 269488144UL, 538976288UL, 1077952576UL, 2155905152UL, 4311810305UL, 8623620610UL, 17247241220UL, 34494482440UL, 68988964880UL, 137977929760UL, 275955859520UL, 551911719040UL, 1103823438081UL, 2207646876162UL, 4415293752324UL, 8830587504648UL, 17661175009296UL, 35322350018592UL, 70644700037184UL, 141289400074368UL, 282578800148737UL, 565157600297474UL, 1130315200594948UL, 2260630401189896UL, 4521260802379792UL, 9042521604759584UL, 18085043209519168UL, 36170086419038336UL };
        private static readonly ulong[] left = new ulong[64] { 0UL, 1UL, 3UL, 7UL, 15UL, 31UL, 63UL, 127UL, 0UL, 256UL, 768UL, 1792UL, 3840UL, 7936UL, 16128UL, 32512UL, 0UL, 65536UL, 196608UL, 458752UL, 983040UL, 2031616UL, 4128768UL, 8323072UL, 0UL, 16777216UL, 50331648UL, 117440512UL, 251658240UL, 520093696UL, 1056964608UL, 2130706432UL, 0UL, 4294967296UL, 12884901888UL, 30064771072UL, 64424509440UL, 133143986176UL, 270582939648UL, 545460846592UL, 0UL, 1099511627776UL, 3298534883328UL, 7696581394432UL, 16492674416640UL, 34084860461056UL, 69269232549888UL, 139637976727552UL, 0UL, 281474976710656UL, 844424930131968UL, 1970324836974592UL, 4222124650659840UL, 8725724278030336UL, 17732923532771328UL, 35747322042253312UL, 0UL, 72057594037927936UL, 216172782113783808UL, 504403158265495552UL, 1080863910568919040UL, 2233785415175766016UL, 4539628424389459968UL, 9151314442816847872UL };
        private static readonly ulong[] right = new ulong[64] { 254UL, 252UL, 248UL, 240UL, 224UL, 192UL, 128UL, 0UL, 65024UL, 64512UL, 63488UL, 61440UL, 57344UL, 49152UL, 32768UL, 0UL, 16646144UL, 16515072UL, 16252928UL, 15728640UL, 14680064UL, 12582912UL, 8388608UL, 0UL, 4261412864UL, 4227858432UL, 4160749568UL, 4026531840UL, 3758096384UL, 3221225472UL, 2147483648UL, 0UL, 1090921693184UL, 1082331758592UL, 1065151889408UL, 1030792151040UL, 962072674304UL, 824633720832UL, 549755813888UL, 0UL, 279275953455104UL, 277076930199552UL, 272678883688448UL, 263882790666240UL, 246290604621824UL, 211106232532992UL, 140737488355328UL, 0UL, 71494644084506624UL, 70931694131085312UL, 69805794224242688UL, 67553994410557440UL, 63050394783186944UL, 54043195528445952UL, 36028797018963968UL, 0UL, 18302628885633695744UL, 18158513697557839872UL, 17870283321406128128UL, 17293822569102704640UL, 16140901064495857664UL, 13835058055282163712UL, 9223372036854775808UL, 0UL };
        public static readonly ulong[] knight = new ulong[64] { 132096UL, 329728UL, 659712UL, 1319424UL, 2638848UL, 5277696UL, 10489856UL, 4202496UL, 33816580UL, 84410376UL, 168886289UL, 337772578UL, 675545156UL, 1351090312UL, 2685403152UL, 1075839008UL, 8657044482UL, 21609056261UL, 43234889994UL, 86469779988UL, 172939559976UL, 345879119952UL, 687463207072UL, 275414786112UL, 2216203387392UL, 5531918402816UL, 11068131838464UL, 22136263676928UL, 44272527353856UL, 88545054707712UL, 175990581010432UL, 70506185244672UL, 567348067172352UL, 1416171111120896UL, 2833441750646784UL, 5666883501293568UL, 11333767002587136UL, 22667534005174272UL, 45053588738670592UL, 18049583422636032UL, 145241105196122112UL, 362539804446949376UL, 725361088165576704UL, 1450722176331153408UL, 2901444352662306816UL, 5802888705324613632UL, 11533718717099671552UL, 4620693356194824192UL, 288234782788157440UL, 576469569871282176UL, 1224997833292120064UL, 2449995666584240128UL, 4899991333168480256UL, 9799982666336960512UL, 1152939783987658752UL, 2305878468463689728UL, 1128098930098176UL, 2257297371824128UL, 4796069720358912UL, 9592139440717824UL, 19184278881435648UL, 38368557762871296UL, 4679521487814656UL, 9077567998918656UL };
        public static readonly ulong[] king = new ulong[64] { 770UL, 1797UL, 3594UL, 7188UL, 14376UL, 28752UL, 57504UL, 49216UL, 197123UL, 460039UL, 920078UL, 1840156UL, 3680312UL, 7360624UL, 14721248UL, 12599488UL, 50463488UL, 117769984UL, 235539968UL, 471079936UL, 942159872UL, 1884319744UL, 3768639488UL, 3225468928UL, 12918652928UL, 30149115904UL, 60298231808UL, 120596463616UL, 241192927232UL, 482385854464UL, 964771708928UL, 825720045568UL, 3307175149568UL, 7718173671424UL, 15436347342848UL, 30872694685696UL, 61745389371392UL, 123490778742784UL, 246981557485568UL, 211384331665408UL, 846636838289408UL, 1975852459884544UL, 3951704919769088UL, 7903409839538176UL, 15806819679076352UL, 31613639358152704UL, 63227278716305408UL, 54114388906344448UL, 216739030602088448UL, 505818229730443264UL, 1011636459460886528UL, 2023272918921773056UL, 4046545837843546112UL, 8093091675687092224UL, 16186183351374184448UL, 13853283560024178688UL, 144959613005987840UL, 362258295026614272UL, 724516590053228544UL, 1449033180106457088UL, 2898066360212914176UL, 5796132720425828352UL, 11592265440851656704UL, 4665729213955833856UL };

        public static ulong Moves(PieceType pieceType, Side side, byte position, Bitboard b)
        {
            if (b.doublecheck && pieceType != PieceType.King)
            {
                return 0ul; //Double check means that king must move
            }
            ulong result = 0ul;

            ulong myside = side == Side.White ? b.WhitePieces : b.BlackPieces;
            ulong attackedSquares = side == Side.White ? b.BlackAttackedSquares : b.WhiteAttackedSquares;

            ulong moves = 0ul;
            switch (pieceType)
            {
                case PieceType.Knight:
                    moves = knight[position];
                    result = ((moves ^ myside) & moves) & b.squares_to_block_check;
                    break;
                case PieceType.Rook:
                    result = RookMoves(position, b, side);
                    break;
                case PieceType.King:
                    moves = king[position];
                    ulong legal_no_takes = moves & (FULL ^ myside);
                    return (legal_no_takes & (FULL ^ attackedSquares)) | CastleMoves(side, b);
                case PieceType.Pawn:
                    result = PawnMoves(side, position, b) & b.squares_to_block_check;
                    break;
                case PieceType.Bishop:
                    result = BishopMoves(position, b, side);
                    break;
                case PieceType.Queen:
                    result = RookMoves(position, b, side) | BishopMoves(position, b, side);
                    break;
            }
            ulong xrays = side == Side.White ? b.b_xrays : b.w_xrays;
            ulong bitpos = (1ul << position);
            if ((bitpos & xrays) != 0) //Are we pinned to something
            {
                foreach (var ray in b.xrays)
                {
                    if ((bitpos & ray) != 0)
                    {
                        result = result & ray;
                        break;
                    }
                }
            }
            return result;
        }

        const ulong W_KINGSIDE_SQUARES = 96UL;
        const ulong W_QUEENSIDE_SQUARES = 14UL;

        const ulong B_KINGSIDE_SQUARES = 6917529027641081856UL;
        const ulong B_QUEENSIDE_SQUARES = 1008806316530991104UL;
        private static ulong CastleMoves(Side side, Bitboard b)
        {
            if (b.check || b.doublecheck)
            {
                return 0ul;
            }

            ulong result = 0ul;

            if (side == Side.White)
            {
                if (b.W_KingsideCastle && (b.WhitePieces & W_KINGSIDE_SQUARES) == 0  && (W_KINGSIDE_SQUARES & b.BlackAttackedSquares) == 0) //Can we castle, and is the space clear?
                {
                    result |= b.W_King << 2;
                }
                if (b.W_QueensideCastle && (b.WhitePieces & W_QUEENSIDE_SQUARES) == 0 && (W_QUEENSIDE_SQUARES & b.BlackAttackedSquares) == 0) //Can we castle, and is the space clear?
                {
                    result |= b.W_King >> 2;
                }
            }
            else
            {
                if (b.B_KingsideCastle && (b.BlackPieces & B_KINGSIDE_SQUARES) == 0 && (B_KINGSIDE_SQUARES & b.WhiteAttackedSquares) == 0) //Can we castle, and is the space clear?
                {
                    result |= b.B_King << 2;
                }
                if (b.B_QueensideCastle && (b.BlackPieces & B_QUEENSIDE_SQUARES) == 0 && (B_QUEENSIDE_SQUARES & b.WhiteAttackedSquares) == 0) //Can we castle, and is the space clear?
                {
                    result |= b.B_King >> 2;
                }
            }
            return result;
        }

        public static ulong[] SlidingAttackRays(PieceType pieceType, Side side, byte position, Bitboard b)
        {
            ulong myside = side == Side.White ? b.WhitePieces : b.BlackPieces;
            switch (pieceType)
            {
                case PieceType.Rook:
                    return RookAttackRays(position, b, side);
                case PieceType.Bishop:
                    return BishopAttackRays(position, b, side);
                case PieceType.Queen:
                    return QueenAttackRays(position, b, side);
                
            }
            return null; //wot
        }
        public static ulong StaticAttackRays(PieceType pieceType, Side side, byte position, Bitboard b)
        {
            switch (pieceType)
            {
                case PieceType.Pawn:
                    return PawnAttackRays(side, position);
                case PieceType.Knight:
                    return knight[position];
                case PieceType.King:
                    return king[position];
            }
            return 0; //wot
        }

        public static ulong PawnAttackRays(Side side, ulong bitboard)
        {
            var attacks = bitboard;
            if (side == Side.White)
            {
                attacks &= NO_TOP_ROW; //Pawns on the top row cannot attack
                attacks = attacks << 7; //This removes the original position, and adds the next one
                attacks |= attacks << 2; //This adds the next attacking position
            }
            else
            {
                attacks &= NO_BOTTOM_ROW; //Pawns on the top row cannot attack
                attacks = attacks >> 7; //This removes the original position, and adds the next one
                attacks |= attacks >> 2; //This adds the next attacking position
            }
            return attacks;
        }
        static ulong KNIGHT_LEFT = 18229723555195321596ul;
        static ulong KNIGHT_FAR_LEFT = 18374403900871474942ul;

        static ulong KNIGHT_RIGHT     = 4557430888798830399ul;
        static ulong KNIGHT_FAR_RIGHT = 9187201950435737471ul;

        static ulong KNIGHT_BOTTOM = 18446744073709486080ul;
        static ulong KNIGHT_FAR_BOTTOM = 18446744073709551360ul;

        static ulong KNIGHT_TOP = 281474976710655ul;
        static ulong KNIGHT_FAR_TOP = 72057594037927935ul;
        public static ulong KnightAttackRays(Side side, ulong bitboard)
        {
            var bottom_right_attacks = (bitboard & KNIGHT_FAR_RIGHT & KNIGHT_BOTTOM) >> 15;
            bottom_right_attacks |= (bitboard & KNIGHT_RIGHT & KNIGHT_FAR_BOTTOM) >> 6;

            var bottom_left_attacks = (bitboard & KNIGHT_FAR_LEFT & KNIGHT_BOTTOM) >> 17;
            bottom_left_attacks |= (bitboard & KNIGHT_LEFT & KNIGHT_FAR_BOTTOM) >> 10;



            var top_right_attacks = (bitboard & KNIGHT_FAR_RIGHT & KNIGHT_TOP) << 17;
            top_right_attacks |= (bitboard & KNIGHT_RIGHT & KNIGHT_FAR_TOP) << 10;

            var top_left_attacks = (bitboard & KNIGHT_FAR_LEFT & KNIGHT_TOP) << 15;
            top_left_attacks |= (bitboard & KNIGHT_LEFT & KNIGHT_FAR_TOP) << 6;


            return bottom_right_attacks | bottom_left_attacks | top_right_attacks | top_left_attacks;
        }

        private static ulong BishopMoves(byte position, Bitboard b, Side s)
        {
            ulong theirpieces;
            ulong mypieces;
            if (s == Side.White)
            {
                mypieces = b.WhitePieces;
                theirpieces = b.BlackPieces;
            }
            else
            {
                mypieces = b.BlackPieces;
                theirpieces = b.WhitePieces;
            }

            var rightupmask = upRight[position];
            ulong uprightBlockers = (rightupmask & mypieces);
            uprightBlockers |= ((theirpieces & rightupmask) & NO_TOP_ROW)<<9;
            uprightBlockers &= (~uprightBlockers) + 1;
            ulong rightBetween = (uprightBlockers - 1) & rightupmask;

            if (uprightBlockers == 0)
            {
                rightBetween = upRight[position];
            }

            var leftupmask = upLeft[position];
            ulong upleftBlockers = (leftupmask & mypieces);
            upleftBlockers |= ((theirpieces & leftupmask) & NO_TOP_ROW) << 7;
            upleftBlockers &= (~upleftBlockers) + 1;
            ulong upleftBetween = (upleftBlockers - 1) & leftupmask;

            if (upleftBlockers == 0)
            {
                upleftBetween = upLeft[position];
            }

            var rightdownmask = downRight[position];
            ulong downrightBlockers = (rightdownmask & mypieces);
            downrightBlockers |= ((theirpieces & rightdownmask) & NO_BOTTOM_ROW) >> 7;
            downrightBlockers = HSB(downrightBlockers);
            ulong rightdownBetween = (FULL ^ ((downrightBlockers<<1) - 1)) & rightdownmask;

            if (downrightBlockers == 0)
            {
                rightdownBetween = downRight[position];
            }

            var leftdownmask = downLeft[position];
            ulong downleftBlockers = (leftdownmask & mypieces);
            downleftBlockers |= ((theirpieces & leftdownmask) & NO_BOTTOM_ROW) >> 9;
            downleftBlockers = HSB(downleftBlockers);
            ulong leftdownBetween = (FULL ^ ((downleftBlockers<<1) - 1)) & leftdownmask;

            if (downleftBlockers == 0)
            {
                leftdownBetween = downLeft[position];
            }

            return b.squares_to_block_check & (rightBetween | upleftBetween | rightdownBetween | leftdownBetween);
        }

        private static ulong[] BishopAttackRays(byte position, Bitboard b, Side s)
        {
            ulong blockers;
            if (s == Side.White)
            {
                blockers = b.WhitePieces;
                blockers |= (b.BlackPieces ^ b.B_King);
            }
            else
            {
                blockers = b.BlackPieces;
                blockers |= (b.WhitePieces ^ b.W_King);
            }

            var rightupmask = upRight[position];
            ulong uprightBlockers = ((blockers & rightupmask) & NO_TOP_ROW) << 9;
            uprightBlockers &= (~uprightBlockers) + 1;
            ulong rightupBetween = (uprightBlockers - 1) & rightupmask;

            if (uprightBlockers == 0)
            {
                rightupBetween = upRight[position];
            }

            var leftupmask = upLeft[position];
            ulong upleftBlockers = ((blockers & leftupmask) & NO_TOP_ROW) << 7;
            upleftBlockers &= (~upleftBlockers) + 1;
            ulong leftupBetween = (upleftBlockers - 1) & leftupmask;

            if (upleftBlockers == 0)
            {
                leftupBetween = upLeft[position];
            }

            var rightdownmask = downRight[position];
            ulong downrightBlockers = ((blockers & rightdownmask) & NO_BOTTOM_ROW) >> 7;
            downrightBlockers = HSB(downrightBlockers);
            ulong rightdownBetween = (FULL ^ ((downrightBlockers<<1) - 1)) & rightdownmask;

            if (downrightBlockers == 0)
            {
                rightdownBetween = downRight[position];
            }

            var leftdownmask = downLeft[position];
            ulong downleftBlockers = ((blockers & leftdownmask) & NO_BOTTOM_ROW) >> 9;
            downleftBlockers = HSB(downleftBlockers);
            ulong leftdownBetween = (FULL ^ ((downleftBlockers<<1) - 1)) & leftdownmask;

            if (downleftBlockers == 0)
            {
                leftdownBetween = downLeft[position];
            }

            return new ulong[4] {rightupBetween, leftupBetween, rightdownBetween, leftdownBetween};
        }
        public static ulong[] QueenAttackRays(byte position, Bitboard b, Side s)
        {
            ulong blockers;
            if (s == Side.White)
            {
                blockers = b.WhitePieces;
                blockers |= (b.BlackPieces ^ b.B_King);
            }
            else
            {
                blockers = b.BlackPieces;
                blockers |= (b.WhitePieces ^ b.W_King);
            }

            var rightmask = right[position];
            ulong rightBlockers = ((blockers & rightmask) & NO_RIGHT_COLLUMN) << 1;
            rightBlockers &= (~rightBlockers) + 1;
            ulong rightBetween = (rightBlockers - 1) & rightmask;

            if (rightBlockers == 0)
            {
                rightBetween = right[position];
            }

            var leftmask = left[position];
            ulong leftBlockers = ((blockers & leftmask) & NO_LEFT_COLLUMN) >> 1;
            leftBlockers = HSB(leftBlockers);
            ulong leftBetween = (((1ul << position) - 1) ^ ((leftBlockers << 1) - 1)) & leftmask;

            if (leftBlockers == 0)
            {
                leftBetween = left[position];
            }

            var upmask = up[position];
            ulong upBlockers = ((blockers & upmask) & NO_TOP_ROW) << 8;
            upBlockers &= (~upBlockers) + 1;
            ulong upBetween = upmask & (upBlockers - 1);

            if (upBlockers == 0)
            {
                upBetween = up[position];
            }

            var downmask = down[position];
            ulong downBlockers = ((blockers & downmask) & NO_BOTTOM_ROW) >> 8;
            downBlockers = HSB(downBlockers);
            ulong downBetween = downmask ^ (downmask & ((downBlockers << 1) - 1));

            if (downBlockers == 0)
            {
                downBetween = down[position];
            }

            //Bishop
            var rightupmask = upRight[position];
            ulong uprightBlockers = ((blockers & rightupmask) & NO_TOP_ROW) << 9;
            uprightBlockers &= (~uprightBlockers) + 1;
            ulong rightupBetween = (uprightBlockers - 1) & rightupmask;

            if (uprightBlockers == 0)
            {
                rightupBetween = upRight[position];
            }

            var leftupmask = upLeft[position];
            ulong upleftBlockers = ((blockers & leftupmask) & NO_TOP_ROW) << 7;
            upleftBlockers &= (~upleftBlockers) + 1;
            ulong leftupBetween = (upleftBlockers - 1) & leftupmask;

            if (upleftBlockers == 0)
            {
                leftupBetween = upLeft[position];
            }

            var rightdownmask = downRight[position];
            ulong downrightBlockers = ((blockers & rightdownmask) & NO_BOTTOM_ROW) >> 7;
            downrightBlockers = HSB(downrightBlockers);
            ulong rightdownBetween = (FULL ^ ((downrightBlockers << 1) - 1)) & rightdownmask;

            if (downrightBlockers == 0)
            {
                rightdownBetween = downRight[position];
            }

            var leftdownmask = downLeft[position];
            ulong downleftBlockers = ((blockers & leftdownmask) & NO_BOTTOM_ROW) >> 9;
            downleftBlockers = HSB(downleftBlockers);
            ulong leftdownBetween = (FULL ^ ((downleftBlockers << 1) - 1)) & leftdownmask;

            if (downleftBlockers == 0)
            {
                leftdownBetween = downLeft[position];
            }


            return new ulong[8] { rightBetween, leftBetween, upBetween, downBetween, rightupBetween, leftupBetween, rightdownBetween, leftdownBetween};
        }

        private static ulong[] Bishop_XRAY(byte position, Bitboard b, Side s)
        {
            ulong theirpieces;
            ulong mypieces;
            if (s == Side.White)
            {
                mypieces = b.WhitePieces;
                theirpieces = b.B_King;
            }
            else
            {
                mypieces = b.BlackPieces;
                theirpieces = b.W_King;
            }

            var rightupmask = upRight[position];
            ulong uprightBlockers = (rightupmask & mypieces);
            uprightBlockers |= ((theirpieces & rightupmask) & NO_TOP_ROW) << 9;
            uprightBlockers &= (~uprightBlockers) + 1;
            ulong rightupBetween = (uprightBlockers - 1) & rightupmask;

            if (uprightBlockers == 0)
            {
                rightupBetween = upRight[position];
            }

            var leftupmask = upLeft[position];
            ulong upleftBlockers = (leftupmask & mypieces);
            upleftBlockers |= ((theirpieces & leftupmask) & NO_TOP_ROW) << 7;
            upleftBlockers &= (~upleftBlockers) + 1;
            ulong leftupBetween = (upleftBlockers - 1) & leftupmask;

            if (upleftBlockers == 0)
            {
                leftupBetween = upLeft[position];
            }

            var rightdownmask = downRight[position];
            ulong downrightBlockers = (rightdownmask & mypieces);
            downrightBlockers |= ((theirpieces & rightdownmask) & NO_BOTTOM_ROW) >> 7;
            downrightBlockers = HSB(downrightBlockers);
            ulong rightdownBetween = (FULL ^ (downrightBlockers << 1 - 1)) & rightdownmask;

            if (downrightBlockers == 0)
            {
                rightdownBetween = downRight[position];
            }

            var leftdownmask = downLeft[position];
            ulong downleftBlockers = (leftdownmask & mypieces);
            downleftBlockers |= ((theirpieces & leftdownmask) & NO_BOTTOM_ROW) >> 9;
            downleftBlockers = HSB(downleftBlockers);
            ulong leftdownBetween = (FULL ^ (downleftBlockers << 1 - 1)) & leftdownmask;

            if (downleftBlockers == 0)
            {
                leftdownBetween = downLeft[position];
            }

            return new ulong[4] { rightupBetween, leftupBetween, rightdownBetween, leftdownBetween };
        }

        public static ulong[] Queen_XRAY(byte position, Bitboard b, Side s)
        {
            ulong theirpieces;
            ulong mypieces;
            if (s == Side.White)
            {
                mypieces = b.WhitePieces;
                theirpieces = b.B_King;
            }
            else
            {
                mypieces = b.BlackPieces;
                theirpieces = b.W_King;
            }

            var rightmask = right[position];
            ulong rightBlockers = rightmask & mypieces;
            rightBlockers |= ((theirpieces & rightmask) & NO_RIGHT_COLLUMN) << 1;
            rightBlockers &= (~rightBlockers) + 1;
            ulong rightBetween = (rightBlockers - 1) & rightmask;

            if (rightBlockers == 0)
            {
                rightBetween = right[position];
            }

            var leftmask = left[position];
            ulong leftBlockers = leftmask & mypieces;
            leftBlockers |= ((theirpieces & leftmask) & NO_LEFT_COLLUMN) >> 1;
            leftBlockers = HSB(leftBlockers);
            ulong leftBetween = (((1ul << position) - 1) ^ ((leftBlockers << 1) - 1)) & leftmask;

            if (leftBlockers == 0)
            {
                leftBetween = left[position];
            }

            var upmask = up[position];
            ulong upBlockers = upmask & mypieces;
            upBlockers |= ((theirpieces & upmask) & NO_TOP_ROW) << 8;
            upBlockers &= (~upBlockers) + 1;
            ulong upBetween = upmask & (upBlockers - 1);

            if (upBlockers == 0)
            {
                upBetween = up[position];
            }

            var downmask = down[position];
            ulong downBlockers = downmask & mypieces;
            downBlockers |= ((theirpieces & downmask) & NO_BOTTOM_ROW) >> 8;
            downBlockers = HSB(downBlockers);
            ulong downBetween = downmask ^ (downmask & ((downBlockers << 1) - 1));

            if (downBlockers == 0)
            {
                downBetween = down[position];
            }

            //Bishop
            var rightupmask = upRight[position];
            ulong uprightBlockers = (rightupmask & mypieces);
            uprightBlockers |= ((theirpieces & rightupmask) & NO_TOP_ROW) << 9;
            uprightBlockers &= (~uprightBlockers) + 1;
            ulong rightupBetween = (uprightBlockers - 1) & rightupmask;

            if (uprightBlockers == 0)
            {
                rightupBetween = upRight[position];
            }

            var leftupmask = upLeft[position];
            ulong upleftBlockers = (leftupmask & mypieces);
            upleftBlockers |= ((theirpieces & leftupmask) & NO_TOP_ROW) << 7;
            upleftBlockers &= (~upleftBlockers) + 1;
            ulong leftupBetween = (upleftBlockers - 1) & leftupmask;

            if (upleftBlockers == 0)
            {
                leftupBetween = upLeft[position];
            }

            var rightdownmask = downRight[position];
            ulong downrightBlockers = (rightdownmask & mypieces);
            downrightBlockers |= ((theirpieces & rightdownmask) & NO_BOTTOM_ROW) >> 7;
            downrightBlockers = HSB(downrightBlockers);
            ulong rightdownBetween = (FULL ^ ((downrightBlockers << 1) - 1)) & rightdownmask;

            if (downrightBlockers == 0)
            {
                rightdownBetween = downRight[position];
            }

            var leftdownmask = downLeft[position];
            ulong downleftBlockers = (leftdownmask & mypieces);
            downleftBlockers |= ((theirpieces & leftdownmask) & NO_BOTTOM_ROW) >> 9;
            downleftBlockers = HSB(downleftBlockers);
            ulong leftdownBetween = (FULL ^ ((downleftBlockers << 1) - 1)) & leftdownmask;

            if (downleftBlockers == 0)
            {
                leftdownBetween = downLeft[position];
            }

            return new ulong[8] { rightBetween, leftBetween, upBetween, downBetween, rightupBetween, leftupBetween, leftdownBetween, rightdownBetween};
        }
        public static ulong enpassantes = 0;
        private static ulong PawnMoves(Side side, byte position, Bitboard b) //TODO enpassant
        {
            ulong passiveMoves = 0ul;
            ulong attackMoves;
            if (side == Side.White)
            {
                attackMoves = whitePawnAttack[position] & b.BlackPieces; //Get all attacking moves where they intersect with black pieces

                passiveMoves = whitePawnNoAttack[position] ^ (whitePawnNoAttack[position] & (b.BlackPieces | b.WhitePieces)); //Get all moves where no pieces intersect

                //On the second row, there will be 2 moves upwards, if a piece is one above, the 2nd move up SHOULD NOT be returned

                if (passiveMoves >> 16 == 1ul << position) //Is there one remaining two rows above
                                                           //This means that there is a piece above the pawn
                {
                    passiveMoves = 0ul; //There shouldn't be any moves here
                }
                if (position / 8 == 4) //Are we on the 5th rank? 
                {
                    if (b.enpassent == (position % 8) + 1) //Left enpassante
                    {
                        attackMoves |= (1ul << position + 9);
                        ++enpassantes;
                    }
                    else if (b.enpassent == (position % 8) - 1)
                    {
                        attackMoves |= (1ul << position + 7);
                        ++enpassantes;
                    }
                }
            }
            else
            {
                attackMoves = blackPawnAttack[position] & b.WhitePieces; //Get all attacking moves where they intersect with white pieces

                passiveMoves = blackPawnNoAttack[position] ^ (blackPawnNoAttack[position] & (b.BlackPieces | b.WhitePieces)); //Get all moves where no pieces intersect

                //On the seventh row, there will be 2 moves fowards(down, >>8), if a piece is one above(below), the 2nd move up SHOULD NOT be returned

                if (passiveMoves << 16 == 1ul << position) //Is there one remaining two rows below
                                                           //This means that there is a piece above the pawn
                {
                    passiveMoves = 0ul; //There shouldn't be any moves here
                }

                if (position / 8 == 3) //Are we on the 4th rank? 
                {
                    if (b.enpassent == (position % 8) + 1) //Left enpassante
                    {
                        attackMoves |= ((1ul << position) >> 7);
                        ++enpassantes;
                    }
                    else if (b.enpassent == (position % 8) - 1)
                    {
                        attackMoves |= ((1ul<<position) >> 9);
                        ++enpassantes;
                    }
                }
            }
            return attackMoves | passiveMoves;
        }

        public static ulong RookMoves(byte position, Bitboard b, Side s)
        {
            ulong theirpieces;
            ulong mypieces;
            if (s == Side.White)
            {
                mypieces = b.WhitePieces;
                theirpieces = b.BlackPieces;
            }
            else
            {
                mypieces = b.BlackPieces;
                theirpieces = b.WhitePieces;
            }

            var rightmask = right[position];
            ulong rightBlockers = rightmask & mypieces;
            rightBlockers |= ((theirpieces & rightmask) & NO_RIGHT_COLLUMN) << 1;
            rightBlockers &= (~rightBlockers) + 1;
            ulong rightBetween = (rightBlockers-1) & rightmask;

            if (rightBlockers == 0)
            {
                rightBetween = right[position];
            }

            var leftmask = left[position];
            ulong leftBlockers = leftmask & mypieces;
            leftBlockers |= ((theirpieces & leftmask) & NO_LEFT_COLLUMN) >> 1;
            leftBlockers = HSB(leftBlockers);
            ulong leftBetween = (((1ul<<position) - 1)^((leftBlockers<<1)-1)) & leftmask;

            if (leftBlockers == 0)
            {
                leftBetween = left[position];
            }

            var upmask = up[position];
            ulong upBlockers = upmask & mypieces;
            upBlockers |= ((theirpieces & upmask) & NO_TOP_ROW) << 8;
            upBlockers &= (~upBlockers) + 1;
            ulong upBetween = upmask & (upBlockers - 1);

            if (upBlockers == 0)
            {
                upBetween = up[position];
            }

            var downmask = down[position]; 
            ulong downBlockers = downmask & mypieces;
            downBlockers |= ((theirpieces & downmask) & NO_BOTTOM_ROW) >> 8;
            downBlockers = HSB(downBlockers);
            ulong downBetween = downmask ^ (downmask & ((downBlockers<<1)-1));

            if (downBlockers == 0)
            {
                downBetween = down[position];
            }

            return b.squares_to_block_check & (rightBetween | leftBetween | upBetween | downBetween);
        }

        public static ulong[] RookAttackRays(byte position, Bitboard b, Side s)
        {
            ulong blockers;
            if (s == Side.White)
            {
                blockers = b.WhitePieces;
                blockers |= (b.BlackPieces ^ b.B_King);
            }
            else
            {
                blockers = b.BlackPieces;
                blockers |= (b.WhitePieces ^ b.W_King);
            }

            var rightmask = right[position];
            ulong rightBlockers = ((blockers & rightmask) & NO_RIGHT_COLLUMN) << 1;
            rightBlockers &= (~rightBlockers) + 1;
            ulong rightBetween = (rightBlockers - 1) & rightmask;

            if (rightBlockers == 0)
            {
                rightBetween = right[position];
            }

            var leftmask = left[position];
            ulong leftBlockers = ((blockers & leftmask) & NO_LEFT_COLLUMN) >> 1;
            leftBlockers = HSB(leftBlockers);
            ulong leftBetween = (((1ul << position) - 1) ^ ((leftBlockers << 1) - 1)) & leftmask;

            if (leftBlockers == 0)
            {
                leftBetween = left[position];
            }

            var upmask = up[position];
            ulong upBlockers = ((blockers & upmask) & NO_TOP_ROW) << 8;
            upBlockers &= (~upBlockers) + 1;
            ulong upBetween = upmask & (upBlockers - 1);

            if (upBlockers == 0)
            {
                upBetween = up[position];
            }

            var downmask = down[position];
            ulong downBlockers = ((blockers & downmask) & NO_BOTTOM_ROW) >> 8;
            downBlockers = HSB(downBlockers);
            ulong downBetween = downmask ^ (downmask & ((downBlockers << 1) - 1));

            if (downBlockers == 0)
            {
                downBetween = down[position];
            }

            return new ulong[4] {rightBetween, leftBetween, upBetween, downBetween};
        }
        public static ulong[] Rook_XRAY(byte position, Bitboard b, Side s)
        {
            ulong theirpieces;
            ulong mypieces;
            if (s == Side.White)
            {
                mypieces = b.WhitePieces;
                theirpieces = b.B_King; ;
            }
            else
            {
                mypieces = b.BlackPieces;
                theirpieces = b.W_King; ;
            }

            var rightmask = right[position];
            ulong rightBlockers = rightmask & mypieces;
            rightBlockers |= ((theirpieces & rightmask) & NO_RIGHT_COLLUMN) << 1;
            rightBlockers &= (~rightBlockers) + 1;
            ulong rightBetween = (rightBlockers - 1) & rightmask;

            if (rightBlockers == 0)
            {
                rightBetween = right[position];
            }

            var leftmask = left[position];
            ulong leftBlockers = leftmask & mypieces;
            leftBlockers |= ((theirpieces & leftmask) & NO_LEFT_COLLUMN) >> 1;
            leftBlockers = HSB(leftBlockers);
            ulong leftBetween = (((1ul << position) - 1) ^ ((leftBlockers << 1) - 1)) & leftmask;

            if (leftBlockers == 0)
            {
                leftBetween = left[position];
            }

            var upmask = up[position];
            ulong upBlockers = upmask & mypieces;
            upBlockers |= ((theirpieces & upmask) & NO_TOP_ROW) << 8;
            upBlockers &= (~upBlockers) + 1;
            ulong upBetween = upmask & (upBlockers - 1);

            if (upBlockers == 0)
            {
                upBetween = up[position];
            }

            var downmask = down[position];
            ulong downBlockers = downmask & mypieces;
            downBlockers |= ((theirpieces & downmask) & NO_BOTTOM_ROW) >> 8;
            downBlockers = HSB(downBlockers);
            ulong downBetween = downmask ^ (downmask & ((downBlockers << 1) - 1));

            if (downBlockers == 0)
            {
                downBetween = down[position];
            }

            return new ulong[4] { rightBetween, leftBetween, upBetween, downBetween };
        }

        internal static ulong[] XRAY(byte position, Bitboard bitboard, Side s, PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.Rook:
                    return Rook_XRAY(position, bitboard, s);
                case PieceType.Bishop:
                    return Bishop_XRAY(position, bitboard, s);
                case PieceType.Queen:
                    return Queen_XRAY(position, bitboard, s);
            }
            return null;
        }
        private static ulong HSB(ulong n)
        {
            n |= (n >> 1);
            n |= (n >> 2);
            n |= (n >> 4);
            n |= (n >> 8);
            n |= (n >> 16);
            n |= (n >> 32);
            n += 1;
            return n >> 1;
        }

        public static double MoveCountTime = 0;
        internal static ulong MoveCount(Bitboard b, Side s)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ulong result = 0ul;

            ulong pawns;
            ulong rooks;
            ulong knights;
            ulong bishops;
            ulong queens;
            ulong king;
            if (s == Side.White)
            {
                pawns = b.W_Pawn;
                rooks = b.W_Rook;
                knights = b.W_Knight;
                bishops = b.W_Bishop;
                queens = b.W_Queen;
                king = b.W_King;
            }
            else
            {
                pawns = b.B_Pawn;
                rooks = b.B_Rook;
                knights = b.B_Knight;
                bishops = b.B_Bishop;
                queens = b.B_Queen;
                king = b.B_King;
            }
            
            result += (GetMoveCount(b, pawns, s, PieceType.Pawn));
            result += (GetMoveCount(b, knights, s, PieceType.Knight));
            result += (GetMoveCount(b, bishops, s, PieceType.Bishop));
            result += (GetMoveCount(b, rooks, s, PieceType.Rook));
            result += (GetMoveCount(b, queens, s, PieceType.Queen));
            result += (GetMoveCount(b, king, s, PieceType.King));

            stopwatch.Stop();
            MoveCountTime += stopwatch.ElapsedTicks;
            return result;
        }
        public static ulong GetMoveCount(Bitboard b, ulong piece_bitboard, Side s, PieceType pieceType)
        {
            ulong result = 0ul;
            while (piece_bitboard != 0)
            {
                byte lsb = (byte)(BitOperations.TrailingZeros(piece_bitboard) - 1);
                ulong bitpos = 1ul << lsb;
                piece_bitboard ^= bitpos; //remove this piece from the ulong of pieces

                ulong moves = MoveGenerator.Moves(pieceType, s, lsb, b);
                result += BitOperations.NumberOfSetBits(moves);
            }
            return result;
        }


        public static double TOTALTIME;
        public static double list_time;
        public static double range_time;

        public static List<Move> CalculateAll(Bitboard b, Side s)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ulong pawns;
            ulong rooks;
            ulong knights;
            ulong bishops;
            ulong queens;
            ulong king;
            if (s == Side.White)
            {
                pawns = b.W_Pawn;
                rooks = b.W_Rook;
                knights = b.W_Knight;
                bishops = b.W_Bishop;
                queens = b.W_Queen;
                king = b.W_King;
            }
            else
            {
                pawns   = b.B_Pawn;
                rooks   = b.B_Rook;
                knights = b.B_Knight;
                bishops = b.B_Bishop;
                queens  = b.B_Queen;
                king    = b.B_King;
            }
            List<Move> result = new List<Move>();
            
            result.AddRange(GetMoves(b, pawns, s, PieceType.Pawn));
            result.AddRange(GetMoves(b, knights, s, PieceType.Knight));
            result.AddRange(GetMoves(b, bishops, s, PieceType.Bishop));
            result.AddRange(GetMoves(b, rooks, s, PieceType.Rook));
            result.AddRange(GetMoves(b, queens, s, PieceType.Queen));
            result.AddRange(GetKingMoves(b, king, s));
            
            stopwatch.Stop();
            TOTALTIME += stopwatch.ElapsedTicks;
            return result;
        }
        public static double GetMovesTime = 0;
        public static double GetMovesCalls = 0;
        public static List<Move> GetMoves(Bitboard b, ulong piece_bitboard, Side s, PieceType pieceType)
        {
            ++GetMovesCalls;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<Move> result = new List<Move>();
            while (piece_bitboard != 0)
            {
                byte lsb = (byte)(BitOperations.TrailingZeros(piece_bitboard) - 1);
                ulong bitpos = 1ul << lsb;
                piece_bitboard ^= bitpos; //remove this piece from the ulong of pieces

                ulong moves = MoveGenerator.Moves(pieceType, s, lsb, b);
                if (moves == 0)
                {
                    continue;
                }
                AddMoves(ref result, lsb, moves, pieceType);
            }
            stopwatch.Stop();
            GetMovesTime += stopwatch.ElapsedTicks;
            return result;
        }
        public static double KingMovesTime = 0;
        public static double KingMovesCalls = 0;
        /*public static List<Move> GetKingMoves(Bitboard b, ulong piece_bitboard, Side s)
        {
            ++KingMovesCalls;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ulong attackedSquares = s == Side.White ? b.BlackAttackedSquares : b.WhiteAttackedSquares;
            ulong myside = s == Side.White ? b.WhitePieces : b.BlackPieces;

            List<Move> result = new List<Move>();
            byte kingpos = (byte)(BitOperations.TrailingZeros(piece_bitboard) - 1);

            var moves = king[kingpos];
            moves &= (moves ^ myside ^ attackedSquares);
            if (moves != 0)
            {
                AddMoves(ref result, kingpos, moves, PieceType.King);
            }
            stopwatch.Stop();
            KingMovesTime += stopwatch.ElapsedTicks;
            return result;
        }*/
        public static List<Move> GetKingMoves(Bitboard b, ulong piece_bitboard, Side s)
        {
            ++KingMovesCalls;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ulong attackedSquares = s == Side.White ? b.BlackAttackedSquares : b.WhiteAttackedSquares;
            ulong myside = s == Side.White ? b.WhitePieces : b.BlackPieces;

            List<Move> result = new List<Move>();
            byte kingpos = (byte)(BitOperations.TrailingZeros(piece_bitboard) - 1);

            ulong moves = king[kingpos];
            ulong legal_no_takes = moves & (FULL ^ myside);
            moves = (legal_no_takes & (FULL ^ attackedSquares));


            var pos = MoveLeft(kingpos);
            if ((pos & moves) != 0)
            {
                result.Add(new Move(kingpos, (byte)(kingpos + 1), PieceType.King));
            }

            pos = MoveRight(kingpos);
            if ((pos & moves) != 0)
            {
                result.Add(new Move(kingpos, (byte)(kingpos - 1), PieceType.King));
            }

            pos = MoveUp(kingpos);
            if ((pos & moves) != 0)
            {
                result.Add(new Move(kingpos, (byte)(kingpos + 8), PieceType.King));
            }

            pos = MoveDown(kingpos);
            if ((pos & moves) != 0)
            {
                result.Add(new Move(kingpos, (byte)(kingpos - 8), PieceType.King));
            }

            pos = MoveUpLeft(kingpos);
            if ((pos & moves) != 0)
            {
                result.Add(new Move(kingpos, (byte)(kingpos + 9), PieceType.King));
            }

            pos = MoveUpRight(kingpos);
            if ((pos & moves) != 0)
            {
                result.Add(new Move(kingpos, (byte)(kingpos + 7), PieceType.King));
            }

            pos = MoveDownLeft(kingpos);
            if ((pos & moves) != 0)
            {
                result.Add(new Move(kingpos, (byte)(kingpos - 7), PieceType.King));
            }

            pos = MoveDownRight(kingpos);
            if ((pos & moves) != 0)
            {
                result.Add(new Move(kingpos, (byte)(kingpos - 9), PieceType.King));
            }

            ulong castle = CastleMoves(s, b);
            if ((castle & (1ul << kingpos + 2)) != 0)
            {
                result.Add(new Move(kingpos, (byte)(kingpos + 2), PieceType.King));
            }
            if ((castle & (1ul << kingpos - 2)) != 0)
            {
                result.Add(new Move(kingpos, (byte)(kingpos - 2), PieceType.King));
            }

            stopwatch.Stop();
            KingMovesTime += stopwatch.ElapsedTicks;
            return result;
        }
        
        private static ulong MoveLeft(byte kingpos)
        {
            return (1ul << (kingpos - 1));
        }
        private static ulong MoveRight(byte kingpos)
        {
            return (1ul << (kingpos + 1));
        }
        private static ulong MoveUp(byte kingpos)
        {
            return (1ul << (kingpos + 8));
        }
        private static ulong MoveDown(byte kingpos)
        {
            return (1ul << (kingpos - 8));
        } //Up is down and down is up
        /*
         *  0 0 0 0 0 0 0 0
            0 0 0 0 0 0 0 0
            0 0 0 0 0 0 0 0
            0 0 0 0 0 0 0 0
            0 0 0 0 0 0 0 0
            1 0 0 0 0 0 0 0
            0 0 0 0 0 0 0 0
            0 0 0 0 0 0 0 0
        * When king moves left by one (<<-1 for flip) , the one will appear on the right hand side
        *  0 0 0 0 0 0 0 0
           0 0 0 0 0 0 0 0
           0 0 0 0 0 0 0 0
           0 0 0 0 0 0 0 0
           0 0 0 0 0 0 0 0
           0 0 0 0 0 0 0 0
           0 0 0 0 0 0 0 1
           0 0 0 0 0 0 0 0

        Therefore, by going & with

         1 1 1 1 1 1 1 0
         1 1 1 1 1 1 1 0
         1 1 1 1 1 1 1 0
         1 1 1 1 1 1 1 0
         1 1 1 1 1 1 1 0
         1 1 1 1 1 1 1 0
         1 1 1 1 1 1 1 0
         1 1 1 1 1 1 1 0

        The one will disappear, it will be a 0 move, or 'invalid'
         */


        private static ulong MoveUpLeft(byte kingpos)
        {
            return (1ul << (kingpos + 7));
        }
        private static ulong MoveUpRight(byte kingpos)
        {
            return (1ul << (kingpos + 9));
        }
        private static ulong MoveDownLeft(byte kingpos)
        {
            return (1ul << (kingpos - 9));
        }
        private static ulong MoveDownRight(byte kingpos)
        {
            return (1ul << (kingpos - 7));
        }

        private static void AddMoves(ref List<Move> result, byte startpos, ulong moves, PieceType pieceType)
        {
            while (moves != 0ul)
            {
                byte lsb = (byte)(BitOperations.TrailingZeros(moves) - 1);
                ulong bitpos = 1ul << lsb;
                moves ^= bitpos; //remove this move from the ulong of moves
                result.Add(new Move(startpos, lsb, pieceType));
            }
        }
    }
    public struct Move
    {
        public byte last;
        public byte current;
        public PieceType pieceType;

        public Move(byte last, byte current, PieceType pieceType)
        {
            this.last = last;
            this.current = current;
            this.pieceType = pieceType;
        }
    }
}